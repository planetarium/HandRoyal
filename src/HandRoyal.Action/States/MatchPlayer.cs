using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class MatchPlayer : IEquatable<MatchPlayer>
{
    [Property(0)]
    public required int PlayerIndex { get; init; }

    [Property(1)]
    public required ImmutableArray<Address> ActiveGloves { get; init; }

    public bool Equals(MatchPlayer? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
