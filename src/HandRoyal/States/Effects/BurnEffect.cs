using HandRoyal.Enums;
using HandRoyal.Gloves;

namespace HandRoyal.States.Effects;

public class BurnEffect(int damagePerRound) : IEffect
{
    public EffectType EffectType => EffectType.Burn;

    public int Duration => -1;

    public (IEffect NextEffect, Condition NextCondition) Apply(Condition condition)
    {
        // Duration will not decrease
        return (this, condition with { HealthPoint = condition.HealthPoint - damagePerRound });
    }

    public EffectData ToEffectData()
    {
        return new EffectData
        {
            Type = EffectType,
            Duration = Duration,
            Parameters = [damagePerRound],
        };
    }
}
