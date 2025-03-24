using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Serialization;
using Libplanet.Crypto;
using HandRoyal.Gloves;
using HandRoyal.Gloves.Effects;

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
    public ImmutableArray<Address> Gloves { get; init; }

    [Property(4)]
    public ImmutableArray<EffectData> ActiveEffects { get; init; }

    public bool HasBurnEffect() =>
        ActiveEffects.Any(e => e.Type == EffectType.Burn);

    public int GetBurnDamage()
    {
        var burnEffect = ActiveEffects.FirstOrDefault(e => e.Type == EffectType.Burn);
        return burnEffect != null ? Convert.ToInt32(burnEffect.Parameters["damagePerRound"]) : 0;
    }

    public bool Equals(Condition? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
