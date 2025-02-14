using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.BlockActions;

internal sealed class ProcessSession : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var height = context.BlockIndex;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        var sessions = Session.GetSessions(world);
        for (var i = 0; i < sessions.Length; i++)
        {
            var session = sessions[i];
            if (session.ProcessRound(height, context.GetRandom()) is { } nextSession)
            {
                var sessionId = session.Metadata.Id;
                sessions[i] = nextSession;
                sessionsAccount = sessionsAccount.SetState(sessionId, nextSession.Bencoded);
            }
        }

        return world.SetAccount(Addresses.Sessions, sessionsAccount);
    }
}
