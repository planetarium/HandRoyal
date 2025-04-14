using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Effects;

public class BurnEffect(int damagePerRound) : IEffect
{
    public EffectType EffectType => EffectType.Burn;

    public int Duration => -1;

    public int DamagePerRound => damagePerRound;

    public (IEffect NextEffect, PlayerContext NextContext)
        Apply(PlayerContext context, BattleContext battleContext)
    {
        // Duration will not decrease
        return (this, context with { HealthPoint = context.HealthPoint - damagePerRound });
    }
}
