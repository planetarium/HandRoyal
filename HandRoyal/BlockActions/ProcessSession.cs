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
        if (sessionsAccount.GetState(Addresses.SessionAddresses) is not List sessionAddresses)
        {
            return world;
        }

        for (var i = 0; i < sessionAddresses.Count; i++)
        {
            var sesstionAddress = new Address(sessionAddresses[i]);
            if (sessionsAccount.GetState(sesstionAddress) is not { } sessionState)
            {
                continue;
            }

            var session = new Session(sessionState);
            session = session.ProcessRound(height, context.GetRandom());
            if (session.State == SessionState.Ended)
            {
                sessionsAccount = sessionsAccount.RemoveState(sesstionAddress);
                var archivedSessionsAccount = world.GetAccount(Addresses.ArchivedSessions);
                archivedSessionsAccount = archivedSessionsAccount.SetState(
                    sesstionAddress, session.Bencoded);
                world = world.SetAccount(Addresses.ArchivedSessions, archivedSessionsAccount);
            }
            else
            {
                sessionsAccount = sessionsAccount.SetState(
                    sesstionAddress, session.Bencoded);
            }
        }

        world = world.SetAccount(Addresses.Sessions, sessionsAccount);
        return world;
    }
}
