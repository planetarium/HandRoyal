using Bencodex.Types;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Mutations;

internal sealed class MutationController(IBlockChainService blockChainService) : GraphController
{
    [MutationRoot("CreateUser")]
    public TxId CreateUser(PrivateKey privateKey)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var createUser = new CreateUser();
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: GetActionValues(createUser));
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [MutationRoot("CreateSession")]
    public TxId CreateSession(
        PrivateKey privateKey,
        Address sessionId,
        Address prize,
        int maximumUser,
        int minimumUser,
        int remainingUser,
        long roundInterval,
        long waitingInterval)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var createSession = new CreateSession
        {
            SessionId = sessionId,
            Prize = prize,
            MaximumUser = maximumUser,
            MinimumUser = minimumUser,
            RemainingUser = remainingUser,
            RoundInterval = roundInterval,
            WaitingInterval = waitingInterval,
        };
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: GetActionValues(createSession));
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [MutationRoot("RegisterGlove")]
    public TxId RegisterGlove(PrivateKey privateKey, Address gloveId)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var registerGlove = new RegisterGlove
        {
            Id = gloveId,
        };
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: GetActionValues(registerGlove));
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [MutationRoot("JoinSession")]
    public TxId JoinSession(PrivateKey privateKey, Address sessionId, Address? gloveId)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
            Glove = gloveId ?? default,
        };
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: GetActionValues(joinSession));
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [MutationRoot("SubmitMove")]
    public TxId SubmitMove(PrivateKey privateKey, Address sessionId, MoveType move)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            Move = move,
        };
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: GetActionValues(submitMove));
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [MutationRoot("StageTransaction")]
    public TxId StageTransaction(HexValue payload)
    {
        var blockChain = blockChainService.BlockChain;
        var tx = Transaction.Deserialize(payload);
        if (!blockChain.StageTransaction(tx))
        {
            throw new InvalidOperationException("Failed to stage transaction.");
        }

        return tx.Id;
    }

    private static IValue[] GetActionValues(params IAction[] actions)
        => [.. actions.Select(action => action.PlainValue)];
}
