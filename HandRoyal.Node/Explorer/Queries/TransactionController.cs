using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Node.Explorer.Types;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Node.Explorer.Queries;

public sealed class TransactionController(
    IBlockChainService blockChainService,
    IActionService actionService,
    IStoreService storeService) : GraphController
{
    [Query("UnsignedTransaction")]
    public HexValue UnsignedTransaction(
        PublicKey publicKey, HexValue plainValue, long nonce, FavValue? maxGasPrice)
    {
        var blockChain = blockChainService.BlockChain;
        var actionLoader = actionService.ActionLoader;
        var bencodedValue = new Bencodex.Codec().Decode(plainValue);
        var action = actionLoader.LoadAction(0, bencodedValue);

        var invoice = new TxInvoice(
            genesisHash: blockChain.Genesis.Hash,
            actions: new TxActionList([action.PlainValue]),
            gasLimit: null,
            maxGasPrice: null);
        var metaData = new TxSigningMetadata(publicKey, nonce);
        var unsignedTransaction = new UnsignedTx(invoice, metaData);
        return unsignedTransaction.SerializeUnsignedTx().ToArray();
    }

    [Query("SignTransaction")]
    public HexValue SignTransaction(HexValue unsignedTransaction, HexValue signature)
    {
        var unsignedTx = TxMarshaler.DeserializeUnsignedTx(unsignedTransaction);
        var signedTransaction = new Transaction(unsignedTx, signature.ToImmutableArray());
        return signedTransaction.Serialize();
    }

    [Query("TransactionResult")]
    public TxResultValue TransactionResult(TxId txId)
    {
        var blockChain = blockChainService.BlockChain;
        var store = storeService.Store;

        try
        {
            var blockHashes = store
                .IterateTxIdBlockHashIndex(txId)
                .Where(blockChain.ContainsBlock)
                .ToArray();
            var block = blockHashes.Length != 0
                ? blockChain[blockHashes[0]]
                : null;

            if (block is not null)
            {
                if (blockChain.GetTxExecution(block.Hash, txId) is { } execution)
                {
                    return new TxResultValue
                    {
                        TxStatus = execution.Fail ? TxStatus.Failure : TxStatus.Success,
                        BlockIndex = block.Index,
                        ExceptionNames = execution.ExceptionNames is { } exceptionNames
                            ? [.. exceptionNames] : null,
                    };
                }
                else
                {
                    return new TxResultValue
                    {
                        TxStatus = TxStatus.Included,
                        BlockIndex = block.Index,
                    };
                }
            }
            else
            {
                return blockChain.GetStagedTransactionIds().Contains(txId)
                    ? new TxResultValue { TxStatus = TxStatus.Staging }
                    : new TxResultValue { TxStatus = TxStatus.Invalid };
            }
        }
        catch (Exception)
        {
            return new TxResultValue { TxStatus = TxStatus.Invalid };
        }
    }
}
