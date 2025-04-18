using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Round
{
    [Property(0)]
    public required Player Player1 { get; set; }

    [Property(1)]
    public required Player Player2 { get; set; }

    [Property(2)]
    public required RoundRuleData RoundRuleData { get; set; }

    /// <summary>
    /// Gets the index of the winner of the round. If -2, round still processing, -1 drawn.
    /// </summary>
    [Property(3)]
    public int Winner { get; init; } = -2;
}
