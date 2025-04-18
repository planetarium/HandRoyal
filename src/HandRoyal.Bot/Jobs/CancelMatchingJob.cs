using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class CancelMatchingJob : JobBase
{
    protected override void Verify(IJobContext context)
    {
        if (!context.Properties.TryGetValue<Options>(out _))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        await context.CancelMatchingAsync(cancellationToken);
        context.LogInformation("Cancelled matching: {BotId}", context.Address);
    }

    protected override void OnFinished(IJobContext context, Exception? exception)
    {
        context.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
    }
}
