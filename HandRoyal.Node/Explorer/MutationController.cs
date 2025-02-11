using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Node.Explorer.Types;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Node.Explorer;

public sealed class MutationController(IBlockChainService blockChainService) : GraphController
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
            actions: [createUser.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id;
    }

    [MutationRoot("CreateSession")]
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

    [MutationRoot("RegisterGlove")]
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

    [MutationRoot("JoinSession")]
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

    [MutationRoot("SubmitMove")]
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
}
