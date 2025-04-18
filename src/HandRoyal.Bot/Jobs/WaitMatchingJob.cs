using HandRoyal.Bot.GraphQL;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class WaitMatchingJob : JobBase
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
        var options = (Options)context.Properties[typeof(Options)];
        var timeout = options.GetRandomTimeout();
        using var cts1 = new CancellationTokenSource(timeout);
        using var cts2 = CancellationTokenSource.CreateLinkedTokenSource(
            cts1.Token, cancellationToken);
        while (!cts2.IsCancellationRequested)
        {
            var user = await context.UpdateUserDataAsync(cts2.Token);
            if (user.SessionId != default)
            {
                context.Properties[typeof(WaitSessionJob.Options)] = new WaitSessionJob.Options
                {
                    State = SessionState.Active,
                };
                context.Properties[typeof(SubmitMoveJob.Options)] = new SubmitMoveJob.Options();
                break;
            }

            await Task.Delay(options.Interval, cts2.Token);
        }

        context.LogInformation("User data updated");
    }

    protected override void OnFinished(IJobContext context, Exception? exception)
    {
        if (exception is not null)
        {
            context.Properties[typeof(CancelMatchingJob.Options)] = new CancelMatchingJob.Options();
        }

        context.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
        public TimeSpan MinimumTimeout { get; init; } = TimeSpan.FromSeconds(5);

        public TimeSpan MaximumTimeout { get; init; } = TimeSpan.FromSeconds(30);

        public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(1);

        public TimeSpan GetRandomTimeout()
        {
            var random = new Random();
            var timeout = MinimumTimeout + TimeSpan.FromMilliseconds(
                random.NextDouble() * (MaximumTimeout - MinimumTimeout).TotalMilliseconds);
            return timeout;
        }
    }
}
