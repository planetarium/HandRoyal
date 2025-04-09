namespace HandRoyal.Bot;

public sealed class JobFinishedEventArgs : JobEventArgs
{
    public Exception? Exception { get; init; }
}
