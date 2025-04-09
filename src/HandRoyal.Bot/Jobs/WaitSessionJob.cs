using HandRoyal.Bot.GraphQL;
using HandRoyal.Bot.Properties;
using HandRoyal.States;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class WaitSessionJob(ILogger<WaitSessionJob> logger)
    : JobBase("WaitSession")
{
    protected override void Verify(IBot bot)
    {
        if (!bot.Properties.TryGetValue<User>(out var user))
        {
            throw new InvalidOperationException("User not set");
        }

        if (user.SessionId == default)
        {
            throw new InvalidOperationException("Session not set");
        }

        if (!bot.Properties.Contains(typeof(Options)))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override void OnFinished(IBot bot, Exception? exception)
    {
        bot.Properties.Remove(typeof(Options));
    }

    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var user = (User)bot.Properties[typeof(User)];
        var options = (Options)bot.Properties[typeof(Options)];
        await foreach (var item in bot.OnSessionChangedAsync(user.SessionId, cancellationToken))
        {
            if (item.SessionState >= options.State)
            {
                break;
            }
        }

        logger.LogInformation("Session state changed: {BotId}", bot.Address);
    }

    public sealed record class Options
    {
        public SessionState State { get; init; } = SessionState.Ended;
    }
}
