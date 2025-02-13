using Bencodex.Types;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer.Queries;

public sealed class StateQueryController(IBlockChainService blockChainService) : GraphController
{
    [Query("Sessions")]
    public List<Session> GetSessions()
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();

        var currentSessionAccount = worldState.GetAccountState(Addresses.Sessions);
        if (currentSessionAccount.GetState(Addresses.ActiveSessionAddresses)
            is not List activeSessionAddresses)
        {
            return [];
        }

        return activeSessionAddresses
            .Select(address => currentSessionAccount.GetState(new Address(address)))
            .Where(state => state is not null)
            .Select(state => new Session(state!))
            .ToList();
    }

    [Query("Session")]
    public Session? GetSession(Address sessionId)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();
        var sessionsAccount = worldState.GetAccountState(Addresses.Sessions);
        if (sessionsAccount.GetState(sessionId) is { } currentSessionState)
        {
            return new Session(currentSessionState);
        }

        return null;
    }

    [Query("User")]
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

    [Query("Glove")]
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
