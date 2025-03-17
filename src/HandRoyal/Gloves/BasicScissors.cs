using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BasicScissors : Glove
{
    public override Address Id => new Address("0x0000000000000000000000000000000000000002");

    public override GloveType Type => GloveType.Scissors;
}
