using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Gloves.Behaviors;

public interface IAttackBehavior
{
    (PlayerContext NextAttackerContext, PlayerContext NextDefenderContext) Execute(
        PlayerContext attackerContext,
        PlayerContext defenderContext,
        BattleResult battleResult,
        BattleContext battleContext);
}
