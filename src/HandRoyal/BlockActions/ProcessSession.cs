using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal.BlockActions;

internal sealed class ProcessSession : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var height = context.BlockIndex;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        if (sessionsAccount.GetState(Addresses.ActiveSessionAddresses)
            is not List activeSessionAddresses)
        {
            return world;
        }

        List<IValue> endedSessionAddresses = [];
        for (var i = 0; i < activeSessionAddresses.Count; i++)
        {
            var sessionAddress = new Address(activeSessionAddresses[i]);
            if (sessionsAccount.GetState(sessionAddress) is not { } sessionState)
            {
                continue;
            }

            var session = new Session(sessionState);
            session = session.ProcessRound(height, context.GetRandom());
            if (session.State == SessionState.Ended)
            {
                endedSessionAddresses.Add(sessionAddress.Bencoded);
                var winerIds = session.Players.Where(item => item.State == PlayerState.Won)
                    .Select(item => item.Id)
                    .ToArray();
                var prize = session.Metadata.Prize;
                var usersAccount = world.GetAccount(Addresses.Users);
                foreach (var winnerId in winerIds)
                {
                    if (usersAccount.GetState(winnerId) is not { } userState)
                    {
                        continue;
                    }

                    var user = new User(userState);
                    user = user with { Gloves = user.Gloves.Add(prize) };
                    usersAccount = usersAccount.SetState(winnerId, user.Bencoded);
                }

                world = world.SetAccount(Addresses.Users, usersAccount);
            }

            sessionsAccount = sessionsAccount.SetState(sessionAddress, session.Bencoded);
        }

        var updatedActiveSessionAddresses = activeSessionAddresses.Except(endedSessionAddresses);
        sessionsAccount = sessionsAccount.SetState(
            Addresses.ActiveSessionAddresses,
            new List(updatedActiveSessionAddresses));

        world = world.SetAccount(Addresses.Sessions, sessionsAccount);
        return world;
    }
}
