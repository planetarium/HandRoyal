using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class CancelMatchingJob(ILogger<CancelMatchingJob> logger)
    : JobBase("CancelMatching")
{
    protected override void Verify(IBot bot)
    {
        if (!bot.Properties.TryGetValue<Options>(out _))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        await bot.CancelMatchingAsync(cancellationToken);
        logger.LogInformation("Cancelled matching: {BotId}", bot.Address);
    }

    protected override void OnFinished(IBot bot, Exception? exception)
    {
        bot.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
    }
}
