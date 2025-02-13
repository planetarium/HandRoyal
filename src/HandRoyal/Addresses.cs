using Libplanet.Crypto;

namespace HandRoyal;

public static class Addresses
{
    // Account Addresses
    public static readonly Address Users = new("0000000000000000000000000000000000000000");

    public static readonly Address Gloves = new("0000000000000000000000000000000000000001");

    public static readonly Address Sessions = new("0000000000000000000000000000000000000010");

    // Session Addresses
    public static readonly Address ActiveSessionAddresses
        = new("0000000000000000000000000000000000000000");

    public static readonly Address MortgagePool
        = new("0000000000000000000000000000000000000100");

    public static readonly Address GasPool
        = new("0000000000000000000000000000000000000200");
}
