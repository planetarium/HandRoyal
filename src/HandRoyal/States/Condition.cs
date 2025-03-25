using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Serialization;
using Libplanet.Crypto;

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

    public bool Equals(Condition? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
