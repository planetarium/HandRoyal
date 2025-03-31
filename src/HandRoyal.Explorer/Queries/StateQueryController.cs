using System.Collections.Immutable;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Explorer.Types;
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

    [Query("UserScopedSession")]
    public SessionEventData GetUserScopedSession(Address sessionId, Address userId)
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        var session = Session.GetSession(world, sessionId);
        var sessionEventData = new SessionEventData(session);
        sessionEventData.UserId = userId;
        return sessionEventData;
    }

    [Query("GetUserData")]
    public UserData? GetUserData(Address userId)
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        var usersAccount = world[Addresses.Users];
        if (usersAccount.TryGetValue<User>(userId, out var user))
        {
            var fav = world.GetBalance(userId, Currencies.Royal);
            return new UserData(user, fav);
        }

        return null;
    }

    [Query("GetMatchPool")]
    public MatchingInfo[] GetMatchPool()
    {
        var blockChain = blockChainService.BlockChain;
        var world = new WorldStateContext(blockChain);
        var matchPoolAccount = world[Addresses.MatchPool];
        if (matchPoolAccount.TryGetValue<ImmutableArray<MatchingInfo>>(
                Addresses.MatchPool,
                out var matchingInfos))
        {
            return matchingInfos.ToArray();
        }

        return [];
    }
}
