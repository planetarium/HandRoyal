using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public abstract class Glove
{
    public abstract Address Id { get; }

    public abstract GloveType Type { get; }
}
