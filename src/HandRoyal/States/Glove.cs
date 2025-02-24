using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(0)]
public sealed record class Glove
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public required Address Author { get; init; }
}
