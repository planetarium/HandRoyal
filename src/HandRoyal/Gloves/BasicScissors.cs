using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BasicScissors : IGlove
{
    public Address Id => new Address("0x0000000000000000000000000000000000000002");

    public GloveType Type => GloveType.Scissors;
}
