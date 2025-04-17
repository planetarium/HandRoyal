using Libplanet.Crypto;

namespace HandRoyal.Bot;

public interface IBot : IServiceProvider, IDisposable
{
    event EventHandler<JobEventArgs>? JobUpdated;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    string Name { get; }

    Address Address { get; }

    Uri EndPoint { get; }

    BotPropertyCollection Properties { get; }

    JobInfo JobInfo { get; }

    bool IsRunning { get; }

    byte[] Sign(byte[] message);

    bool Verify(byte[] message, byte[] signature);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
