using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class RegisterMatchingJob(ILogger<RegisterMatchingJob> logger)
    : JobBase("RegisterMatching")
{
    protected override void Verify(IBot bot)
    {
        if (!bot.Properties.Contains(typeof(Options)))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        await bot.RegisterMatchingAsync(cancellationToken);
        bot.Properties[typeof(UserData)] = await bot.GetUserDataAsync(cancellationToken);
        bot.Properties[typeof(WaitMatchingJob.Options)] = new WaitMatchingJob.Options();
        logger.LogInformation("Registered matching: {BotId}", bot.Address);
    }

    protected override void OnFinished(IBot bot, Exception? exception)
    {
        bot.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
    }
}
