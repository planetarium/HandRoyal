using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

public sealed class MatchMadeEventData(Address sessionId, Address[] players)
{
    public Address SessionId => sessionId;

    public Address[] Players => players;
}
