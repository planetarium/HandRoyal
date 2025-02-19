using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Tx;

namespace HandRoyal;

public sealed record TxSettings
{
    public required PrivateKey PrivateKey { get; init; }

    public required IAction[] Actions { get; init; }

    public TxId StageTo(BlockChain blockChain)
    {
        if (Actions.Length == 0)
        {
            throw new InvalidOperationException("Actions array must contain at least one action.");
        }

        var address = PrivateKey.Address;
        var nonce = blockChain.GetNextTxNonce(address);
        var genesisHash = blockChain.Genesis.Hash;
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: PrivateKey,
            genesisHash: genesisHash,
            actions: Actions.Select(item => item.PlainValue));

        if (!blockChain.StageTransaction(tx))
        {
            throw new InvalidOperationException("Failed to stage transaction.");
        }

        return tx.Id;
    }
}
