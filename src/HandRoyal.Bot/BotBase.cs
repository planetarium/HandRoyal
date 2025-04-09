using Libplanet.Crypto;

namespace HandRoyal.Bot;

public abstract class BotBase(BotOptions options)
    : IBot, IJobSelector, IJobObserver
{
    private readonly BotOptions _options = options.EnsureComplete();

    public event EventHandler<JobEventArgs>? JobStarted;

    public event EventHandler<JobFinishedEventArgs>? JobFinished;

    public event EventHandler<JobEventArgs>? JobUpdated;

    public string Name => _options.Name;

    public Address Address => _options.PrivateKey.Address;

    public Uri EndPoint => _options.GraphqlUrl;

    public BotPropertyCollection Properties { get; } = [];

    public JobInfo Job { get; private set; } = JobInfo.Empty;

    Task<Type> IJobSelector.SelectJobAsync(CancellationToken cancellationToken)
        => SelectJobAsync(cancellationToken);

    public byte[] Sign(byte[] message) => _options.PrivateKey.Sign(message);

    public bool Verify(byte[] message, byte[] signature)
        => _options.PrivateKey.PublicKey.Verify(message, signature);

    void IJobObserver.OnJobStarted(Type type, string name)
    {
        Job = Job with
        {
            Type = type,
            Name = name,
            State = "Started",
            StartTime = DateTimeOffset.UtcNow,
            FinishTime = DateTimeOffset.MinValue,
        };
        OnJobStarted(new JobEventArgs { Type = type, Name = name });
    }

    void IJobObserver.OnJobFinished(Type type, string name, Exception? exception)
    {
        Job = Job with
        {
            Type = type,
            Name = name,
            State = exception == null ? "Finished" : "Failed",
            FinishTime = DateTimeOffset.UtcNow,
        };
        OnJobFinished(new JobFinishedEventArgs { Type = type, Name = name, Exception = exception });
    }

    void IJobObserver.OnJobUpdated(Type type, string name, string state)
    {
        Job = Job with
        {
            Type = type,
            Name = name,
            State = state,
        };
        OnJobUpdated(new JobEventArgs { Type = type, Name = name });
    }

    object? IServiceProvider.GetService(Type serviceType)
        => options.ServiceProvider?.GetService(serviceType);

    protected abstract Task<Type> SelectJobAsync(CancellationToken cancellationToken);

    protected virtual void OnJobStarted(JobEventArgs e) => JobStarted?.Invoke(this, e);

    protected virtual void OnJobFinished(JobFinishedEventArgs e) => JobFinished?.Invoke(this, e);

    protected virtual void OnJobUpdated(JobEventArgs e) => JobUpdated?.Invoke(this, e);
}
