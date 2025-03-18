using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Gloves;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Queries;

internal sealed class StateQueryController(IBlockChainService blockChainService) : GraphController
{
    [Query("Sessions")]
    public Session[] GetSessions()
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        return Session.GetSessions(world);
    }

    [Query("Session")]
    public Session? GetSession(Address sessionId)
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        var sessionsAccount = world[Addresses.Sessions];
        if (sessionsAccount.TryGetValue<Session>(sessionId, out var session))
        {
            return session;
        }

        var archivedSessionsAccount = world[Addresses.ArchivedSessions];
        if (archivedSessionsAccount.TryGetValue<Session>(sessionId, out var archivedSession))
        {
            return archivedSession;
        }

        return null;
    }

    [Query("User")]
    public User? GetUser(Address userId)
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        var usersAccount = world[Addresses.Users];
        if (usersAccount.TryGetValue<User>(userId, out var user))
        {
            return user;
        }

        return null;
    }

    [Query("Glove")]
    public IGlove? GetGlove(Address gloveId)
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        var glovesAccount = world[Addresses.Gloves];
        if (glovesAccount.TryGetValue<IGlove>(gloveId, out var glove))
        {
            return glove;
        }

        return null;
    }
}
