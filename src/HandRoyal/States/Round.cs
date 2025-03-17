using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Round
{
    [Property(0)]
    public required Condition Condition1 { get; set; }

    [Property(1)]
    public required Condition Condition2 { get; set; }

    /// <summary>
    /// Gets the index of the winner of the round. If -2, round still processing, -1 drawn.
    /// </summary>
    [Property(2)]
    public int Winner { get; init; }
}
