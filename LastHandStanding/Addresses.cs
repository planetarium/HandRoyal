using Libplanet.Crypto;

namespace LastHandStanding;

public static class Addresses
{
    // Accounts
    public static readonly Address Users  = new("0000000000000000000000000000000000000000");

    public static readonly Address Gloves = new("0000000000000000000000000000000000000001");

    public static readonly Address Sessions = new("0000000000000000000000000000000000000010");

    public static readonly Address CurrentSession = new("0000000000000000000000000000000000000100");

    public static readonly Address ArchivedSessions = new("0000000000000000000000000000000000000101");

    public static readonly Address SessionsList = new("0000000000000000000000000000000000000101");
}