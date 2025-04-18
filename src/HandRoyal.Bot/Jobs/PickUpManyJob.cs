using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class PickUpManyJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        await context.PickUpManyAsync(cancellationToken);
        context.Properties[typeof(UpdateUserJob.Options)] = new UpdateUserJob.Options();
        context.LogInformation("Picked up many: {BotId}", context.Address);
    }
}
