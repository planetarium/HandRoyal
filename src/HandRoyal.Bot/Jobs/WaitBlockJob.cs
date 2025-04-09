using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class WaitBlockJob(ILogger<WaitSessionJob> logger)
    : JobBase("WaitBlock")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        if (!bot.Properties.TryGetValue<Options>(out var _))
        {
            return;
        }

        logger.LogInformation("WaitBlockJob started");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await foreach (var tip in bot.OnTiChangedAsync(cts.Token))
        {
            logger.LogInformation("Tip: {Height} {Hash}", tip.Height, tip.Hash);
        }

        logger.LogInformation("WaitBlockJob finished");
        bot.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
        public long Height { get; init; } = 10;
    }
}
