using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class GloveInfo : IEquatable<GloveInfo>
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public required int Count { get; init; }
}
