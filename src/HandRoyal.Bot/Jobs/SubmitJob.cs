using HandRoyal.States;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class SubmitJob(ILogger<SubmitJob> logger)
    : JobBase("Submit")
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
    }

    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        logger.LogInformation("Submit");
    }

    public sealed record class Options
    {
    }
}
