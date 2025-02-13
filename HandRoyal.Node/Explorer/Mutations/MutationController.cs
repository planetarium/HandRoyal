using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Node.Explorer.Types;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Node.Explorer.Mutations;

public sealed class MutationController(IBlockChainService blockChainService) : GraphController
{
    [Mutation("CreateUser")]
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
            actions: [createUser.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [Mutation("CreateSession")]
    public TxId CreateSession(PrivateKey privateKey, Address sessionId, Address prize)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var createSession = new CreateSession(sessionId, prize);
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: [createSession.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [Mutation("RegisterGlove")]
    public TxId RegisterGlove(PrivateKey privateKey, Address gloveId)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var registerGlove = new RegisterGlove(gloveId);
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: [registerGlove.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [Mutation("JoinSession")]
    public TxId JoinSession(PrivateKey privateKey, Address sessionId, Address? gloveId)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var joinSession = new JoinSession(sessionId, gloveId);
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: [joinSession.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [Mutation("SubmitMove")]
    public TxId SubmitMove(PrivateKey privateKey, Address sessionId, MoveType move)
    {
        var address = privateKey.Address;
        var blockChain = blockChainService.BlockChain;
        var submitMove = new SubmitMove(sessionId, move);
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: blockChain.Genesis.Hash,
            actions: [submitMove.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [Mutation("StageTransaction")]
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
}
