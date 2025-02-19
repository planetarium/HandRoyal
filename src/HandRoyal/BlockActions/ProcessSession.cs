using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.BlockActions;

internal sealed class ProcessSession : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var height = context.BlockIndex;
        var random = context.GetRandom();
        var sessionsAccount = world[Addresses.Sessions];
        var sessions = Session.GetSessions(world);
        for (var i = 0; i < sessions.Length; i++)
        {
            var session = sessions[i];
            if (session.ProcessRound(height, random) is { } nextSession)
            {
                var sessionId = session.Metadata.Id;
                sessionsAccount[sessionId] = nextSession;
            }
        }
    }
}
