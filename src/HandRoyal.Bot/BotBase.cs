using HandRoyal.Pages;
using Libplanet.Crypto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot;

public abstract class BotBase(BotOptions options, ILogger logger)
    : IBot, IJobObserver, ILogSource, IHasLogger
{
    private readonly BotOptions _options = options.EnsureComplete();
    private readonly BotLogger _logger = new(logger);
    private Task _runTask = Task.CompletedTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposedValue;

    public event EventHandler<JobEventArgs>? JobUpdated;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public string Name => _options.Name;

    public Address Address => _options.PrivateKey.Address;

    public Uri EndPoint => _options.GraphqlUrl;

    public BotPropertyCollection Properties { get; } = [];

    public JobInfo JobInfo { get; private set; } = JobInfo.Empty;

    public bool IsRunning { get; private set; }

    ILogger IHasLogger.Logger => _logger;

    public byte[] Sign(byte[] message) => _options.PrivateKey.Sign(message);

    public bool Verify(byte[] message, byte[] signature)
        => _options.PrivateKey.PublicKey.Verify(message, signature);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Already started");
        }

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);

        _runTask = ExecuteJobAsync(this, _cancellationTokenSource.Token);
        IsRunning = true;
        OnStarted(EventArgs.Empty);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("Already stopped");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
        }

        try
        {
            await _runTask;
        }
        catch
        {
            // Ignore exceptions from the task
        }

        _runTask = Task.CompletedTask;
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        Properties.Clear();
        JobInfo = JobInfo.Empty;
        IsRunning = false;
        OnStopped(EventArgs.Empty);
    }

    void IJobObserver.OnStarted(Type type, string name)
    {
        JobInfo = JobInfo with
        {
            Type = type,
            Name = name,
            State = "Started",
            StartTime = DateTimeOffset.UtcNow,
            FinishTime = DateTimeOffset.MinValue,
        };
        OnJobUpdated(new JobEventArgs { JobInfo = JobInfo });
        _logger.LogInformation("Job started: {Type} - {Name}", type.Name, name);
    }

    void IJobObserver.OnFinished(Type type, string name, Exception? exception)
    {
        JobInfo = JobInfo with
        {
            Type = type,
            Name = name,
            State = exception == null ? "Finished" : "Failed",
            FinishTime = DateTimeOffset.UtcNow,
            Exception = exception,
        };
        OnJobUpdated(new JobEventArgs { JobInfo = JobInfo });
        if (exception == null)
        {
            _logger.LogInformation("Job finished: {Type} - {Name}", type.Name, name);
        }
        else
        {
            _logger.LogError(exception, "Job failed: {Type} - {Name}", type.Name, name);
        }
    }

    void IJobObserver.OnUpdated(Type type, string name, string state)
    {
        JobInfo = JobInfo with
        {
            Type = type,
            Name = name,
            State = state,
        };
        OnJobUpdated(new JobEventArgs { JobInfo = JobInfo });
        _logger.LogInformation("Job updated: {Type} - {Name} - {State}", type.Name, name, state);
    }

    object? IServiceProvider.GetService(Type serviceType)
        => options.ServiceProvider?.GetService(serviceType);

    void IDisposable.Dispose()
    {
        OnDispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    IAsyncEnumerable<string> ILogSource.GetLogStream(CancellationToken cancellationToken)
        => _logger.GetLogStream(cancellationToken);

    protected abstract Task<Type> SelectJobAsync(CancellationToken cancellationToken);

    protected virtual void OnJobUpdated(JobEventArgs e) => JobUpdated?.Invoke(this, e);

    protected virtual void OnStarted(EventArgs e) => Started?.Invoke(this, e);

    protected virtual void OnStopped(EventArgs e) => Stopped?.Invoke(this, e);

    protected virtual void OnDispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _runTask = Task.CompletedTask;
            IsRunning = false;
            _disposedValue = true;
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }

    private static async Task ExecuteJobAsync(BotBase bot, CancellationToken cancellationToken)
    {
        var jobService = bot.GetRequiredService<IJobService>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var jobType = await bot.SelectJobAsync(cancellationToken);
            var job = jobService.Jobs[jobType];
            try
            {
                await job.ExecuteAsync(bot, cancellationToken);
            }
            catch
            {
                // Do nothing
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
