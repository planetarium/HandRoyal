using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(0)]
public sealed record class Move
{
    [Property(0)]
    public int PlayerIndex { get; init; }

    [Property(1)]
    public MoveType Type { get; init; }
}
