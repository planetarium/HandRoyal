using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal;

public static class Currencies
{
    public static readonly Currency Royal = Currency.Uncapped("Royal", 18, null);

    public static readonly Currency Gas = Currency.Uncapped("Gas", 18, null);

    public static Address SinkAddress { get; } =
        new Address("0x2c3B8CC04639456Dc886FeA10cd6dAba4117349B");
}
