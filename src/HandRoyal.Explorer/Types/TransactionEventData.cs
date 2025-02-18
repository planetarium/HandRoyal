using GraphQL.AspNet.Attributes;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Types;

internal sealed class TransactionEventData
{
    [GraphSkip]
    public TxId[] TxIds { get; init; } = [];

    public BlockHash BlockHash { get; set; }

    public long BlockHeight { get; set; }

    public TxId TxId { get; set; }

    public TxStatus Status { get; set; }
}
