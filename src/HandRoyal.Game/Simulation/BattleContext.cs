using Libplanet.Action;

namespace HandRoyal.Game.Simulation;

public record BattleContext
{
    public required int RoundIndex { get; init; }

    public required IRandom Random { get; init; }
}
