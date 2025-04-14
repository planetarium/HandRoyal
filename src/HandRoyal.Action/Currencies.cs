using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal;

public static class Currencies
{
    public static readonly Currency Royal = Currency.Uncapped("Royal", 18, null);

    public static readonly Currency Gas = Currency.Uncapped("Gas", 18, null);

    public static Address SinkAddress { get; } =
        new Address("0xfaaEe5D8cd17f9835673C2666a65Af36e431e152");
}
