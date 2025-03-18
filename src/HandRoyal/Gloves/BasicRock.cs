using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BasicRock : IGlove
{
    public Address Id => new Address("0x0000000000000000000000000000000000000000");

    public GloveType Type => GloveType.Rock;
}
