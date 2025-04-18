using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class WaitBlockJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        if (!context.Properties.TryGetValue<Options>(out var _))
        {
            return;
        }

        context.LogInformation("WaitBlockJob started");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await foreach (var tip in context.OnTiChangedAsync(cts.Token))
        {
            context.LogInformation("Tip: {Height} {Hash}", tip.Height, tip.Hash);
        }

        context.LogInformation("WaitBlockJob finished");
        context.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
        public long Height { get; init; } = 10;
    }
}
