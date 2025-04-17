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

    public IBot AddNew()
    {
        if (!_options.IsEnabled)
        {
            throw new InvalidOperationException("Cannot add bot: bot service is disabled.");
        }

        if (!_isRunning)
        {
            throw new InvalidOperationException("Cannot add bot: bot service is not running.");
        }

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var botLogger = loggerFactory.CreateLogger<UserBot>();
        var botOptions = new BotOptions
        {
            Name = GenerateNewName("bot"),
            PrivateKey = new PrivateKey(),
            GraphqlUrl = new Uri(_options.GraphqlEndpoint),
            ServiceProvider = serviceProvider,
        };

        var bot = new UserBot(botOptions, botLogger);
        Bots.Add(bot);
        _ = bot.StartAsync(default);
        return bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("Bot service is already running");
        }

        if (!_options.IsEnabled)
        {
            return;
        }

        _logger.LogInformation("Starting bot service...");
        await Task.CompletedTask;

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
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
            var botLogger = loggerFactory.CreateLogger<UserBot>();
            var bot = new UserBot(botOptions, botLogger)
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
            var botLogger = loggerFactory.CreateLogger<OrganiserBot>();
            var bot = new OrganiserBot(botOptions, botLogger)
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
            _ = bot.StartAsync(_cancellationTokenSource.Token);
        }

        _isRunning = true;
        _logger.LogInformation("All {BotCount} bots launched", _bots.Count);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.IsEnabled)
        {
            return;
        }

        if (!_isRunning)
        {
            throw new InvalidOperationException("Bot service is not running");
        }

        foreach (var bot in _bots)
        {
            if (bot.IsRunning)
            {
                await bot.StopAsync(cancellationToken);
            }
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

    private string GenerateNewName(string name)
    {
        var i = 0;
        var botName = $"{name}{i++}";
        while (_bots.Contains(botName))
        {
            botName = $"{name}{i++}";
        }

        return botName;
    }
}
