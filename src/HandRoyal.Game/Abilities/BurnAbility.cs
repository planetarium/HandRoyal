using HandRoyal.Game.Effects;
using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Abilities;

public class BurnAbility(int damage) : IAbility
{
    public AbilityType AbilityType => AbilityType.Burn;

    public int Duration { get; set;  } = -1;

    public (PlayerContext AttackerContext, PlayerContext DefenderContext) Apply(
        PlayerContext attackerContext,
        PlayerContext defenderContext,
        BattleResult battleResult,
        BattleContext context)
    {
        var nextDefenderContext = defenderContext;
        if (!defenderContext.Effects.Any(effect => effect is BurnEffect))
        {
            nextDefenderContext = nextDefenderContext.AdjustEffect(new BurnEffect(damage));
        }

        return (attackerContext, nextDefenderContext);
    }
}
