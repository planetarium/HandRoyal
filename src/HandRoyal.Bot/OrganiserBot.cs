using HandRoyal.Bot.Jobs;
using HandRoyal.Bot.Properties;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot;

public sealed class OrganiserBot(BotOptions options, ILogger<OrganiserBot> logger)
    : BotBase(options, logger)
{
    protected override Task<Type> SelectJobAsync(CancellationToken cancellationToken)
    {
        if (Properties.Contains(typeof(IdleJob.Options)))
        {
            return Task.FromResult(typeof(IdleJob));
        }

        if (Properties.TryGetValue<SessionInfo>(out var sessionInfo) && sessionInfo.Id != default)
        {
            return Task.FromResult(typeof(WaitSessionJob));
        }
        else
        {
            return Task.FromResult(typeof(CreateSessionJob));
        }
    }
}
