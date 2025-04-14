using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Gloves.Behaviors;

public class LateCommerBehavior : IAttackBehavior
{
    public (PlayerContext NextAttackerContext, PlayerContext NextDefenderContext) Execute(
        PlayerContext attackerContext,
        PlayerContext defenderContext,
        BattleResult battleResult,
        BattleContext battleContext)
    {
        if (battleResult != BattleResult.Win)
        {
            return (attackerContext, defenderContext);
        }

        // 일반적인 가위바위보 로직
        defenderContext =
            defenderContext.ApplyDamage(
                attackerContext.Glove!.BaseDamage * (battleContext.RoundIndex + 1), battleContext);
        return (attackerContext, defenderContext);
    }
}
