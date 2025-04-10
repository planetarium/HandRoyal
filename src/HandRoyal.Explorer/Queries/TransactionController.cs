using System.Text;
using System.Text.Json;
using Bencodex.Json;
using Bencodex.Types;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Explorer.Types;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Queries;

internal sealed class TransactionController(
    IBlockChainService blockChainService,
    IActionService actionService,
    IStoreService storeService) : GraphController
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new BencodexJsonConverter(),
        },
    };

    [Query("UnsignedTransaction")]
    public HexValue UnsignedTransaction(
        Address address, HexValue plainValue, FavValue? maxGasPrice)
    {
        var blockChain = blockChainService.BlockChain;
        var actionLoader = actionService.ActionLoader;
        var bencodedValue = new Bencodex.Codec().Decode(plainValue);
        var action = actionLoader.LoadAction(0, bencodedValue);
        var nonce = blockChain.GetNextTxNonce(address);

        var invoice = new TxInvoice(
            genesisHash: blockChain.Genesis.Hash,
            actions: new TxActionList([action.PlainValue]),
            gasLimit: null,
            maxGasPrice: null);
        var metaData = new TxSigningMetadata(address, nonce);
        var unsignedTransaction = new UnsignedTx(invoice, metaData);
        var json = unsignedTransaction.SerializeUnsignedTx();
        return Encoding.UTF8.GetBytes(json);
    }

    [Query("SignTransaction")]
    public HexValue SignTransaction(HexValue unsignedTransaction, HexValue signature)
    {
        var reader = new Utf8JsonReader((byte[])unsignedTransaction);
        var value = new BencodexJsonConverter().Read(ref reader, typeof(IValue), SerializerOptions)
            ?? throw new InvalidOperationException("Failed to parse the unsigned transaction.");
        var unsignedTx = TxMarshaler.UnmarshalUnsignedTx((Dictionary)value);
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
