using System.Collections.Immutable;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class MatchingInfo
{
    [Property(0)]
    public required Address UserId { get; init; }

    [Property(1)]
    public required ImmutableArray<Address> Gloves { get; init; }

    [Property(2)]
    public required long RegisteredHeight { get; init; }
}
