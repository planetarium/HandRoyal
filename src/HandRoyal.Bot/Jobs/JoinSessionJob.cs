using HandRoyal.Bot.Properties;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class JoinSessionJob(ILogger<JoinSessionJob> logger)
    : JobBase("JoinSession")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        if (!bot.Properties.TryGetValue<SessionInfo>(out var _))
        {
            var sessionIds = await bot.GetSessionsAsync(cancellationToken);
            if (sessionIds.Length == 0)
            {
                return;
            }

            var index = Random.Shared.Next(sessionIds.Length);
            var sessionId = sessionIds[index];
            await bot.JoinSessionAsync(sessionId, cancellationToken);
            bot.Properties[typeof(SessionInfo)] = new SessionInfo(sessionId);
            logger.LogInformation("Joined session: {UserId} {SessionId}", bot.Address, sessionId);
        }
    }
}
