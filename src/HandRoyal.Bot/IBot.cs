using Libplanet.Crypto;

namespace HandRoyal.Bot;

public interface IBot : IServiceProvider
{
    event EventHandler<JobEventArgs>? JobStarted;

    event EventHandler<JobFinishedEventArgs>? JobFinished;

    event EventHandler<JobEventArgs>? JobUpdated;

    string Name { get; }

    Address Address { get; }

    Uri EndPoint { get; }

    BotPropertyCollection Properties { get; }

    JobInfo Job { get; }

    byte[] Sign(byte[] message);

    bool Verify(byte[] message, byte[] signature);
}
