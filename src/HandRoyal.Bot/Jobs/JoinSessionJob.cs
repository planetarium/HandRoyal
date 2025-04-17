using HandRoyal.Bot.Properties;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class JoinSessionJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        if (!context.Properties.TryGetValue<SessionInfo>(out var _))
        {
            var sessionIds = await context.GetJoinableSessionsAsync(cancellationToken);
            if (sessionIds.Length == 0)
            {
                return;
            }

            var index = Random.Shared.Next(sessionIds.Length);
            var sessionId = sessionIds[index];
            await context.JoinSessionAsync(sessionId, cancellationToken);
            context.Properties[typeof(SessionInfo)] = new SessionInfo(sessionId);
            context.LogInformation(
                "Joined session: {UserId} {SessionId}", context.Address, sessionId);
        }
    }
}
