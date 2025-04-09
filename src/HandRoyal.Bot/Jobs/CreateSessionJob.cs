using HandRoyal.Bot.Properties;
using Libplanet.Crypto;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class CreateSessionJob(ILogger<CreateSessionJob> logger)
    : JobBase("CreateSession")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var sessionId = new PrivateKey().Address;
        await bot.CreateSessionAsync(sessionId, cancellationToken);
        bot.Properties[typeof(SessionInfo)] = new SessionInfo(sessionId);
        logger.LogInformation("Created new session: {UserId} {SessionId}", bot.Address, sessionId);
    }
}
