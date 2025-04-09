using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class SessionJob(ILogger<SessionJob> logger)
    : JobBase("Session")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
