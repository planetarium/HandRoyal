using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.Enums;
using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.BlockActions;

internal sealed class PostProcessSession : BlockActionBase
{
    public const int WinReward = 8;

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

            sessionsAccount.Remove(sessionId);
            archivedSessionsAccount[sessionId] = session;
            sessionList.RemoveAt(i);
        }

        sessionsAccount[Addresses.Sessions] = new List(
            sessionList.Select(item => item.Metadata.Id.Bencoded));
    }

    private static void UpdateUsersStates(IWorldContext world, Session session)
    {
        var winnerIds = session.UserEntries
            .Where(item => item.State == UserEntryState.Won)
            .Select(item => item.Id)
            .ToArray();
        var prize = session.Metadata.Prize;
        var usersAccount = world[Addresses.Users];
        var userIds = session.UserEntries.Select(player => player.Id).ToArray();
        foreach (var userId in userIds)
        {
            if (!usersAccount.TryGetValue<User>(userId, out var user))
            {
                continue;
            }

            if (winnerIds.Contains(userId) && prize is { } prizeNotNull)
            {
                user = user.ObtainGlove(prizeNotNull, 1);
            }

            usersAccount[userId] = user with
            {
                SessionId = default,
            };
        }

        // Gold Reward
        foreach (var winner in winnerIds)
        {
            world.TransferAsset(Currencies.SinkAddress, winner, Currencies.Royal * WinReward);
        }
    }
}
