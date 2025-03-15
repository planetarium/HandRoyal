using System.Text;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
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
        long roundInterval)
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
    public TxId JoinSession(PrivateKey privateKey, Address sessionId, Address? gloveId)
    {
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
            Glove = gloveId ?? default,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [joinSession],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("SubmitMove")]
    public TxId SubmitMove(PrivateKey privateKey, Address sessionId, MoveType move)
    {
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            Move = move,
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
