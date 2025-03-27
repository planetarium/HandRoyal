using GraphQL.AspNet.Attributes;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using Libplanet.Crypto;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Types;

public sealed class PickUpResultEventData(TxId txId, string[] gloves)
{
    public TxId TxId => txId;

    public string[] Gloves => gloves;
}
