using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class SessionJob : JobBase
{
    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        context.LogInformation("Session job started");
        await Task.CompletedTask;
    }
}
