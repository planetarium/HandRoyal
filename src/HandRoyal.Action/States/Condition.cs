using System.Collections.Immutable;
using System.Runtime.Serialization;
using HandRoyal.Game.Effects;
using HandRoyal.Game.Gloves;
using HandRoyal.Game.Simulation;
using HandRoyal.Loader;
using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
[KnownType(typeof(BurnEffect))]
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

    internal ImmutableArray<IEffect> ActiveEffects =>
        [..ActiveEffectData.Select(EffectLoader.CreateEffect)];

    public PlayerContext ToPlayerContext(IGlove? glove)
    {
        return new PlayerContext
        {
            HealthPoint = HealthPoint,
            Effects = ActiveEffects,
            Glove = glove,
        };
    }

    public bool Equals(Condition? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
