using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Node.Explorer;

public class BakeryController(IBlockChainService blockChainService) : GraphController
{
    [QueryRoot("IsValidSessionId")]
    public bool IsValidSessionId(string sessionId)
    {
        var address = new Address(sessionId);
        var blockChain = blockChainService.BlockChain;

        var worldState = blockChain.GetWorldState();
        var currentSessionAccount = worldState.GetAccountState(Addresses.CurrentSession);
        if (currentSessionAccount.GetState(address) is not null)
        {
            return false;
        }

        var archivedSessionsAccount = worldState.GetAccountState(Addresses.ArchivedSessions);
        if (archivedSessionsAccount.GetState(address) is not null)
        {
            return false;
        }

        return true;
    }

    [MutationRoot("CreateUser")]
    public string CreateUser(string privateKey)
    {
        var key = new PrivateKey(privateKey);
        var address = key.Address;
        var blockChain = blockChainService.BlockChain;
        var createUser = new CreateUser();
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: key,
            genesisHash: blockChain.Genesis.Hash,
            actions: [createUser.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id.ToString();
    }

    [MutationRoot("CreateSession")]
    public string CreateUser(string privateKey, string sessionId, string prize)
    {
        var key = new PrivateKey(privateKey);
        var address = key.Address;
        var blockChain = blockChainService.BlockChain;
        var createSession = new CreateSession(new Address(sessionId), new Address(prize));
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: key,
            genesisHash: blockChain.Genesis.Hash,
            actions: [createSession.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id.ToString();
    }

    [MutationRoot("RegisterGlove")]
    public string RegisterGlove(string privateKey, string gloveId)
    {
        var key = new PrivateKey(privateKey);
        var address = key.Address;
        var blockChain = blockChainService.BlockChain;
        var registerGlove = new RegisterGlove(new Address(gloveId));
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: key,
            genesisHash: blockChain.Genesis.Hash,
            actions: [registerGlove.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id.ToString();
    }

    [MutationRoot("JoinSession")]
    public string JoinSession(string privateKey, string sessionId, string glove)
    {
        var key = new PrivateKey(privateKey);
        var address = key.Address;
        var blockChain = blockChainService.BlockChain;
        var joinSession = new JoinSession(new Address(sessionId), new Address(glove));
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: key,
            genesisHash: blockChain.Genesis.Hash,
            actions: [joinSession.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id.ToString();
    }

    [MutationRoot("SubmitMove")]
    public string SubmitMove(string privateKey, string sessionId, MoveType move)
    {
        var key = new PrivateKey(privateKey);
        var address = key.Address;
        var blockChain = blockChainService.BlockChain;
        var submitMove = new SubmitMove(new Address(sessionId), move);
        var nonce = blockChain.GetNextTxNonce(address);
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: key,
            genesisHash: blockChain.Genesis.Hash,
            actions: [submitMove.PlainValue]);
        blockChain.StageTransaction(tx);

        return tx.Id.ToString();
    }
}
