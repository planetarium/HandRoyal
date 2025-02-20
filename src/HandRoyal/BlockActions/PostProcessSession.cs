using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.BlockActions;

internal sealed class PostProcessSession : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var sessionsAccount = world[Addresses.Sessions];
        var archivedSessionsAccount = world[Addresses.ArchivedSessions];
        var sessionList = Session.GetSessions(world).ToList();
        for (var i = sessionList.Count - 1; i >= 0; i--)
        {
            var session = sessionList[i];
            var sessionId = session.Metadata.Id;
            if (session.State != SessionState.Ended)
            {
                continue;
            }

            UpdateUsersStates(world, session);

            sessionsAccount.RemoveState(sessionId);
            archivedSessionsAccount[sessionId] = session;
            sessionList.RemoveAt(i);
        }

        sessionsAccount[Addresses.Sessions] = new List(
            sessionList.Select(item => item.Metadata.Id.Bencoded));
    }

    private static void UpdateUsersStates(IWorldContext world, Session session)
    {
        var winerIds = session.Players
            .Where(item => item.State == PlayerState.Won)
            .Select(item => item.Id)
            .ToArray();
        var prize = session.Metadata.Prize;
        var usersAccount = world[Addresses.Users];
        var userIds = session.Players.Select(player => player.Id).ToArray();
        foreach (var userId in userIds)
        {
            if (!usersAccount.TryGetObject<User>(userId, out var user))
            {
                continue;
            }

            var gloves = winerIds.Contains(userId) ? user.Gloves.Add(prize) : user.Gloves;
            usersAccount[userId] = user with
            {
                Gloves = gloves,
                SessionId = default,
            };
        }
    }
}
