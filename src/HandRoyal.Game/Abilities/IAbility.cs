using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Abilities;

public interface IAbility
{
    AbilityType AbilityType { get; }

    (PlayerContext AttackerContext, PlayerContext DefenderContext) Apply(
        PlayerContext attackerContext,
        PlayerContext defenderContext,
        bool isAttackerWin,
        BattleContext context);
}
