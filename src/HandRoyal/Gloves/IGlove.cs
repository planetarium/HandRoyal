using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public interface IGlove
{
    public Address Id { get; }

    public GloveType Type { get; }
}
