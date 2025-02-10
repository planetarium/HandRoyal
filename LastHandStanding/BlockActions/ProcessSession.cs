using Bencodex.Types;
using LastHandStanding.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace LastHandStanding.BlockActions;

internal sealed class ProcessSession : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var height = context.BlockIndex;
        var currentSessionAccount = world.GetAccount(Addresses.CurrentSession);
        if (currentSessionAccount.GetState(Addresses.SessionsList) is not List sessionAddresses)
        {
            return world;
        }

        for (var i = 0; i < sessionAddresses.Count; i++)
        {
            var sesstionAddress = new Address(sessionAddresses[i]);
            if (currentSessionAccount.GetState(sesstionAddress) is not { } sessionState)
            {
                continue;
            }

            var session = new Session(sessionState);
            session = session.ProcessRound(height, context.GetRandom());
            if (session.State == SessionState.Ended)
            {
                currentSessionAccount = currentSessionAccount.RemoveState(sesstionAddress);
                var archivedSessionsAccount = world.GetAccount(Addresses.ArchivedSessions);
                archivedSessionsAccount = archivedSessionsAccount.SetState(
                    sesstionAddress, session.Bencoded);
                world = world.SetAccount(Addresses.ArchivedSessions, archivedSessionsAccount);
            }
            else
            {
                currentSessionAccount = currentSessionAccount.SetState(
                    sesstionAddress, session.Bencoded);
            }
        }

        world = world.SetAccount(Addresses.CurrentSession, currentSessionAccount);
        return world;
    }
}
