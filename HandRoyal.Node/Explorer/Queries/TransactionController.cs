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
    public TxResult TransactionResult(TxId txId)
    {
        var blockChain = blockChainService.BlockChain;
        var store = storeService.Store;

        try
        {
            var blockHashCandidates = store
                .IterateTxIdBlockHashIndex(txId)
                .Where(bhc => blockChain.ContainsBlock(bhc))
                .ToList();
            var blockContainingTx = blockHashCandidates.Count != 0
                ? blockChain[blockHashCandidates.First()]
                : null;

            if (blockContainingTx is { } block)
            {
                if (blockChain.GetTxExecution(block.Hash, txId) is { } execution)
                {
                    return new TxResult
                    {
                        TxStatus = execution.Fail ? TxStatus.FAILURE : TxStatus.SUCCESS,
                        BlockIndex = block.Index,
                        ExceptionNames = execution.ExceptionNames is { } exceptionNames
                            ? exceptionNames.ToArray() : null,
                    };
                }
                else
                {
                    return new TxResult
                    {
                        TxStatus = TxStatus.INCLUDED,
                        BlockIndex = block.Index,
                    };
                }
            }
            else
            {
                return blockChain.GetStagedTransactionIds().Contains(txId)
                    ? new TxResult { TxStatus = TxStatus.STAGING }
                    : new TxResult { TxStatus = TxStatus.INVALID };
            }
        }
        catch (Exception)
        {
            return new TxResult { TxStatus = TxStatus.INVALID };
        }
    }
}
