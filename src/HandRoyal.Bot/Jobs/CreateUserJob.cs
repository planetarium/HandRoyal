using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class CreateUserJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        var userId = context.Address;
        var name = context.Name;
        await context.CreateUserAsync(cancellationToken);
        context.Properties[typeof(UpdateUserJob.Options)] = new UpdateUserJob.Options
        {
        };
        context.LogInformation("Created new user: {UserId} {Name}", userId, name);
    }
}
