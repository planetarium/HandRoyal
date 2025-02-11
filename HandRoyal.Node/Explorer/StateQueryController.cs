using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer;

public sealed class StateQueryController(IBlockChainService blockChainService) : GraphController
{
    [QueryRoot("StateQuery/Session")]
    public Session? GetSession(Address sessionId)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();
        var sessionsAccount = worldState.GetAccountState(Addresses.Sessions);
        if (sessionsAccount.GetState(sessionId) is { } currentSessionState)
        {
            return new Session(currentSessionState);
        }

        var archivedSessionsAccount = worldState.GetAccountState(Addresses.ArchivedSessions);
        if (archivedSessionsAccount.GetState(sessionId) is { } archivedSessionState)
        {
            return new Session(archivedSessionState);
        }

        return null;
    }

    [QueryRoot("StateQuery/User")]
    public User? GetUser(Address userId)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();
        var usersAccount = worldState.GetAccountState(Addresses.Users);
        if (usersAccount.GetState(userId) is { } userState)
        {
            return new User(userState);
        }

        return null;
    }

    [QueryRoot("StateQuery/Glove")]
    public Glove? GetGlove(Address gloveId)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();
        var glovesAccount = worldState.GetAccountState(Addresses.Gloves);
        if (glovesAccount.GetState(gloveId) is { } gloveState)
        {
            return new Glove(gloveState);
        }

        return null;
    }
}
