using System.Collections.Immutable;
using System.Text;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Mutations;

internal sealed class MutationController(IBlockChainService blockChainService) : GraphController
{
    [MutationRoot("CreateUser")]
    public TxId CreateUser(PrivateKey privateKey)
    {
        var createUser = new CreateUser();
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [createUser],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("CreateSession")]
    public TxId CreateSession(
        PrivateKey privateKey,
        Address sessionId,
        Address prize,
        int maximumUser,
        int minimumUser,
        int remainingUser,
        long startAfter,
        int maxRounds,
        long roundLength,
        long roundInterval,
        int initialHealthPoint,
        int numberOfGloves)
    {
        var createSession = new CreateSession
        {
            SessionId = sessionId,
            Prize = prize,
            MaximumUser = maximumUser,
            MinimumUser = minimumUser,
            RemainingUser = remainingUser,
            StartAfter = startAfter,
            MaxRounds = maxRounds,
            RoundLength = roundLength,
            RoundInterval = roundInterval,
            InitialHealthPoint = initialHealthPoint,
            NumberOfGloves = numberOfGloves,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [createSession],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("RegisterGlove")]
    public TxId RegisterGlove(PrivateKey privateKey, Address gloveId)
    {
        var registerGlove = new RegisterGlove
        {
            Id = gloveId,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [registerGlove],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("JoinSession")]
    public TxId JoinSession(PrivateKey privateKey, Address sessionId, IEnumerable<Address> gloves)
    {
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
            Gloves = gloves.ToImmutableArray(),
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [joinSession],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("SubmitMove")]
    public TxId SubmitMove(PrivateKey privateKey, Address sessionId, int gloveIndex)
    {
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            GloveIndex = gloveIndex,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [submitMove],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("StageTransaction")]
    public TxId StageTransaction(HexValue unsignedTransaction, HexValue signature)
    {
        var json = Encoding.UTF8.GetString((byte[])unsignedTransaction);
        var unsignedTx = TxMarshaler.DeserializeUnsignedTxFromJson(json);
        var tx = new Transaction(unsignedTx, signature.ToImmutableArray());
        var blockChain = blockChainService.BlockChain;
        if (!blockChain.StageTransaction(tx))
        {
            throw new InvalidOperationException("Failed to stage transaction.");
        }

        return tx.Id;
    }
}
