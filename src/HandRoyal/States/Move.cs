using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Move
{
    [Property(0)]
    public long BlockIndex { get; init; }

    [Property(1)]
    public int PlayerIndex { get; init; }

    [Property(2)]
    public MoveType Type { get; init; }
}
