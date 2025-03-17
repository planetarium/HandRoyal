using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BasicPaper : Glove
{
    public override Address Id => new Address("0x0000000000000000000000000000000000000001");

    public override GloveType Type => GloveType.Paper;
}
