using HandRoyal.Bot.Properties;
using Libplanet.Crypto;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class CreateSessionJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        var sessionId = new PrivateKey().Address;
        await context.CreateSessionAsync(sessionId, cancellationToken);
        context.Properties[typeof(SessionInfo)] = new SessionInfo(sessionId);
        context.LogInformation(
            "Created new session: {UserId} {SessionId}", context.Address, sessionId);
    }
}
