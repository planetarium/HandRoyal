using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BasicPaper : IGlove
{
    public Address Id => new Address("0x0000000000000000000000000000000000000001");

    public GloveType Type => GloveType.Paper;
}
