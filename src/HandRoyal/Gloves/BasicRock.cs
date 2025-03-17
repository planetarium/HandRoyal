using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BasicRock : Glove
{
    public override Address Id => new Address("0x0000000000000000000000000000000000000000");

    public override GloveType Type => GloveType.Rock;
}
