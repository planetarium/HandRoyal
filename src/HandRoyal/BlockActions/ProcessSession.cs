using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal.BlockActions;

internal sealed class ProcessSession : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var height = context.BlockIndex;
        var sessionsAccount = world[Addresses.Sessions];
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
