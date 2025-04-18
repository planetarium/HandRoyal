using HandRoyal.Bot.GraphQL;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class WaitSessionJob : JobBase
{
    protected override void Verify(IJobContext context)
    {
        if (!context.Properties.TryGetValue<UserData>(out var user))
        {
            throw new InvalidOperationException("User not set");
        }

        if (user.SessionId == default)
        {
            throw new InvalidOperationException("Session not set");
        }

        if (!context.Properties.Contains(typeof(Options)))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override void OnFinished(IJobContext context, Exception? exception)
    {
        context.Properties.Remove(typeof(Options));
    }

    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        var user = (UserData)context.Properties[typeof(UserData)];
        var options = (Options)context.Properties[typeof(Options)];
        await foreach (var item in context.OnSessionChangedAsync(user.SessionId, cancellationToken))
        {
            if (item.SessionState >= options.State)
            {
                break;
            }
        }

        context.LogInformation("Session state changed: {BotId}", context.Address);
    }

    public sealed record class Options
    {
        public SessionState State { get; init; } = SessionState.Ended;
    }
}
