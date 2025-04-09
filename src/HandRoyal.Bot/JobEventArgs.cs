namespace HandRoyal.Bot;

public class JobEventArgs : EventArgs
{
    public required Type Type { get; init; }

    public required string Name { get; init; }
}
