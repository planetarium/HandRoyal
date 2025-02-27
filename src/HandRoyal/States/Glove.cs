using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Glove : StateBase<Glove>
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public required Address Author { get; init; }
}
