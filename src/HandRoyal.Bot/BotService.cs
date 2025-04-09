using System.Threading.Tasks;
using HandRoyal.Bot.Jobs;
using HandRoyal.Bot.Options;
using Libplanet.Crypto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HandRoyal.Bot;

internal sealed class BotService(
    IServiceProvider serviceProvider,
    IJobService jobService,
    ILogger<BotService> logger,
    IOptions<BotServiceOptions> options)
    : IBotService, IHostedService
{
    private readonly ILogger<BotService> _logger = logger;
    private readonly BotServiceOptions _options = options.Value;
    private readonly BotCollection _bots = [];
    private readonly List<Task> _jobTasks = [];
    private bool _isRunning;
    private CancellationTokenSource? _cancellationTokenSource;

    public BotCollection Bots => _bots;

    public bool IsEnabled => _options.IsEnabled;

    public void RegisterBot(IBot bot)
    {
        if (!_options.IsEnabled)
        {
            throw new InvalidOperationException(
                "Cannot register bot: bot service is disabled.");
        }

        if (_isRunning)
        {
            throw new InvalidOperationException(
                "Cannot register bot while the service is running.");
        }

        if (_bots.Count >= _options.BotCount)
        {
            throw new InvalidOperationException(
                $"Cannot register bot: maximum number of bots ({_options.BotCount}) reached");
        }

        if (_bots.Contains(bot.Name))
        {
            throw new InvalidOperationException(
                $"Bot with name '{bot.Name}' is already registered");
        }

        _bots.Add(bot);
        _logger.LogInformation("Bot {BotName} registered", bot.Name);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("Bot service is already running");
        }

        if (!_options.IsEnabled)
        {
            throw new InvalidOperationException("Bot service is disabled.");
        }

        _logger.LogInformation("Starting bot service...");
        await Task.CompletedTask;

        var graphqlUrl = new Uri(_options.GraphqlEndpoint);
        _cancellationTokenSource = new CancellationTokenSource();
        for (var i = 0; i < _options.BotCount; i++)
        {
            var botOptions = new BotOptions
            {
                Name = $"bot{i}",
                PrivateKey = new PrivateKey(),
                GraphqlUrl = graphqlUrl,
                ServiceProvider = serviceProvider,
            };
            var bot = new UserBot(botOptions)
            {
                Properties =
                {
                    new IdleJob.Options { Delay = 3000 },
                },
            };

            _bots.Add(bot);
        }

        for (var i = 0; i < _options.OrganiserBotCount; i++)
        {
            var botOptions = new BotOptions
            {
                Name = $"organiser{i}",
                PrivateKey = new PrivateKey(),
                GraphqlUrl = graphqlUrl,
                ServiceProvider = serviceProvider,
            };
            var bot = new OrganiserBot(botOptions)
            {
                Properties =
                {
                    new IdleJob.Options { Delay = 3000 },
                },
            };

            _bots.Add(bot);
        }

        foreach (var bot in _bots)
        {
            _jobTasks.Add(ExecuteJobAsync(bot, _cancellationTokenSource.Token));
        }

        _isRunning = true;
        _logger.LogInformation("All {BotCount} bots launched", _bots.Count);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.IsEnabled)
        {
            throw new InvalidOperationException("Bot service is disabled.");
        }

        if (!_isRunning)
        {
            throw new InvalidOperationException("Bot service is not running");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            await Task.WhenAll(_jobTasks);
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        _logger.LogInformation("Stopping all bots...");

        _isRunning = false;
        _logger.LogInformation("All bots stopped");
    }

    private static void StartJob(IBot bot, IJob job)
    {
        if (bot is IJobObserver jobObserver)
        {
            jobObserver.OnJobStarted(job.GetType(), job.Name);
        }
    }

    private static void FinishJob(IBot bot, IJob job, Exception? exception)
    {
        if (bot is IJobObserver jobObserver)
        {
            jobObserver.OnJobFinished(job.GetType(), job.Name, exception);
        }
    }

    private async Task<IJob> SelectJobAsync(
        IBot bot, CancellationToken cancellationToken)
    {
        if (bot is IJobSelector jobSelector)
        {
            try
            {
                var jobType = await jobSelector.SelectJobAsync(cancellationToken);
                return jobService.Jobs[jobType];
            }
            catch
            {
                // Do nothing
            }
        }

        return jobService.Jobs[typeof(IdleJob)];
    }

    private async Task ExecuteJobAsync(
        IBot bot, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var job = await SelectJobAsync(bot, cancellationToken);
            try
            {
                StartJob(bot, job);
                await job.ExecuteAsync(bot, cancellationToken);
                FinishJob(bot, job, exception: null);
            }
            catch (Exception e)
            {
                FinishJob(bot, job, e);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
