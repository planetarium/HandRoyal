using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class IdleJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        var delay = GetDelay(context);
        await Task.Delay(delay, cancellationToken);
        context.Properties.Remove(typeof(Options));
        context.LogInformation("Idle job executed");
    }

    private static int GetDelay(IJobContext context)
    {
        if (context.Properties.TryGetValue<Options>(out var options))
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
