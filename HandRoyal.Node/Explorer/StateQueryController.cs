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

        var currentSessionAccount = worldState.GetAccountState(Addresses.Sessions);
        if (currentSessionAccount.GetState(sessionId) is { } currentSessionState)
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
    public User? GetUser(Address userAddress)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();
        var userAccount = worldState.GetAccountState(Addresses.Users);
        if (userAccount.GetState(userAddress) is { } userState)
        {
            return new User(userState);
        }

        return null;
    }
}
