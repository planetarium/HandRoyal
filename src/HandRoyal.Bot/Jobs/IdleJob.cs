using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class IdleJob(ILogger<IdleJob> logger)
    : JobBase("Idle")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var delay = GetDelay(bot);
        await Task.Delay(delay, cancellationToken);
        bot.Properties.Remove(typeof(Options));
        logger.LogInformation("Idle job executed");
    }

    private static int GetDelay(IBot bot)
    {
        if (bot.Properties.TryGetValue<Options>(out var options))
        {
            return options.Delay;
        }

        return 1000;
    }

    public sealed record class Options
    {
        public int Delay { get; init; } = 1000;
    }
}
