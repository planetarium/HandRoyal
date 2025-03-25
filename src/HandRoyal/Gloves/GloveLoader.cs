using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public static class GloveLoader
{
    public static IGlove LoadGlove(Address address) => address.ToHex() switch
    {
        "0000000000000000000000000000000000000000" => new BasicRock(),
        "0000000000000000000000000000000000000001" => new BasicPaper(),
        "0000000000000000000000000000000000000002" => new BasicScissors(),
        _ => throw new ArgumentException($"Invalid glove address '{address}'"),
    };
}
