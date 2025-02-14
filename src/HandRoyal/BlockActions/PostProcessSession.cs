using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.BlockActions;

internal sealed class PostProcessSession : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        var archivedSessionsAccount = world.GetAccount(Addresses.ArchivedSessions);
        var sessionList = Session.GetSessions(world).ToList();
        for (var i = sessionList.Count - 1; i >= 0; i--)
        {
            var session = sessionList[i];
            var sessionId = session.Metadata.Id;
            if (session.State != SessionState.Ended)
            {
                continue;
            }

            var winerIds = session.Players.Where(item => item.State == PlayerState.Won)
                .Select(item => item.Id)
                .ToArray();
            var prize = session.Metadata.Prize;
            var usersAccount = world.GetAccount(Addresses.Users);
            var userIds = session.Players.Select(player => player.Id).ToArray();
            foreach (var userId in userIds)
            {
                if (usersAccount.GetState(userId) is not { } userState)
                {
                    continue;
                }

                var user = new User(userState);
                var gloves = winerIds.Contains(userId) ? user.Gloves.Add(prize) : user.Gloves;
                user = user with
                {
                    Gloves = gloves,
                    SessionId = default,
                };
                usersAccount = usersAccount.SetState(userId, user.Bencoded);
            }

            world = world.SetAccount(Addresses.Users, usersAccount);
            sessionsAccount = sessionsAccount.RemoveState(sessionId);
            archivedSessionsAccount = archivedSessionsAccount.SetState(sessionId, session.Bencoded);
            sessionList.RemoveAt(i);
        }

        sessionsAccount = sessionsAccount.SetState(
            Addresses.Sessions, new List(sessionList.Select(s => s.Metadata.Id.Bencoded)));
        world = world.SetAccount(Addresses.Sessions, sessionsAccount);
        world = world.SetAccount(Addresses.ArchivedSessions, archivedSessionsAccount);
        return world;
    }
}
