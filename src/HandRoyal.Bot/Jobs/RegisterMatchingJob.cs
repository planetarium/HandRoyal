using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class RegisterMatchingJob : JobBase
{
    protected override void Verify(IJobContext context)
    {
        if (!context.Properties.Contains(typeof(Options)))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        await context.RegisterMatchingAsync(cancellationToken);
        context.Properties[typeof(UserData)] = await context.GetUserDataAsync(cancellationToken);
        context.Properties[typeof(WaitMatchingJob.Options)] = new WaitMatchingJob.Options();
        context.LogInformation("Registered matching: {BotId}", context.Address);
    }

    protected override void OnFinished(IJobContext context, Exception? exception)
    {
        context.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
    }
}
