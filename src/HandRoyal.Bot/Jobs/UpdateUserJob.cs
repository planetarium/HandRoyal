using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class UpdateUserJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        if (!context.Properties.TryGetValue<Options>(out _))
        {
            return;
        }

        context.Properties[typeof(UserData)] = await context.GetUserDataAsync(cancellationToken);
        context.Properties.Remove(typeof(Options));
        context.LogInformation("User data updated");
    }

    public sealed record class Options
    {
    }
}
