using System.Collections.Immutable;
using HandRoyal.Game.Abilities;
using HandRoyal.Game.Effects;
using HandRoyal.Game.Gloves;

namespace HandRoyal.Game.Simulation;

public record PlayerContext
{
    public required int HealthPoint { get; init; }

    public required ImmutableArray<IEffect> Effects { get; init; }

    public required IGlove? Glove { get; init; }

    // Maybe move to simulator?
    public PlayerContext ApplyDamage(int damage, BattleContext battleContext)
    {
        var calculatedDamage = damage;
        if (Glove is not null)
        {
            foreach (var effect in Glove!.Abilities)
            {
                if (effect is DamageReductionAbility dre)
                {
                    calculatedDamage = Math.Max(0, calculatedDamage - dre.ReductionAmount);
                }
            }
        }

        return this with { HealthPoint = HealthPoint - calculatedDamage };
    }

    public PlayerContext AdjustEffect(IEffect effect)
    {
        return this with
        {
            Effects = Effects.Add(effect),
        };
    }

    public PlayerContext ApplyEffects(BattleContext battleContext)
    {
        var context = this;
        var nextEffects = new List<IEffect>();
        foreach (var effect in Effects)
        {
            var (nextEffect, nextContext) = effect.Apply(context, battleContext);
            nextEffects.Add(nextEffect);
            context = nextContext;
        }

        return context with { Effects = [..nextEffects] };
    }
}
