using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class PickUpJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        await context.PickUpAsync(cancellationToken);
        context.Properties[typeof(UpdateUserJob.Options)] = new UpdateUserJob.Options();
        context.LogInformation("Picked up: {BotId}", context.Address);
    }
}
