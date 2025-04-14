using Libplanet.Crypto;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Types;

public sealed class PickUpResultEventData(TxId txId, Address userId, string[] gloves)
{
    public TxId TxId => txId;

    public Address UserId => userId;

    public string[] Gloves => gloves;
}
