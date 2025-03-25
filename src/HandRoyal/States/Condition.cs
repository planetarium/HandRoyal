using System.Collections.Immutable;
using HandRoyal.Gloves;
using HandRoyal.Serialization;
using HandRoyal.States.Effects;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Condition : IEquatable<Condition>
{
    [Property(0)]
    public required int HealthPoint { get; init; } = 100;

    [Property(1)]
    public ImmutableArray<bool> GloveUsed { get; init; }

    [Property(2)]
    public int Submission { get; init; } = -1;

    [Property(3)]
    public ImmutableArray<EffectData> ActiveEffectData { get; init; } =
        ImmutableArray<EffectData>.Empty;

    public ImmutableArray<IEffect> ActiveEffects =>
        [..ActiveEffectData.Select(EffectLoader.CreateEffect)];

    public Condition AdjustEffect(IEffect effect)
    {
        return this with
        {
            ActiveEffectData = ActiveEffectData.Add(effect.ToEffectData()),
        };
    }

    public Condition ApplyEffects()
    {
        var condition = this;
        var nextEffects = new List<IEffect>();
        foreach (var effect in ActiveEffects)
        {
            var (nextEffect, nextCondition) = effect.Apply(condition);
            nextEffects.Add(nextEffect);
            condition = nextCondition;
        }

        condition = condition with
        {
            ActiveEffectData = [..nextEffects.Select(effect => effect.ToEffectData())],
        };
        return condition;
    }

    public bool Equals(Condition? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
