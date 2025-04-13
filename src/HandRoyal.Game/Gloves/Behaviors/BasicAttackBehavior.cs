using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Gloves.Behaviors;

public class BasicAttackBehavior : IAttackBehavior
{
    public (PlayerContext NextAttackerContext, PlayerContext NextDefenderContext) Execute(
        PlayerContext attackerContext,
        PlayerContext defenderContext,
        bool isAttackerWin,
        BattleContext battleContext)
    {
        if (!isAttackerWin)
        {
            return (attackerContext, defenderContext);
        }

        // 일반적인 가위바위보 로직
        defenderContext =
            defenderContext.ApplyDamage(attackerContext.Glove!.BaseDamage, battleContext);

        return (attackerContext, defenderContext);
    }
}
