using HandRoyal.Bot.GraphQL;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class WaitMatchingJob(ILogger<WaitMatchingJob> logger)
    : JobBase("WaitMatching")
{
    protected override void Verify(IBot bot)
    {
        if (!bot.Properties.TryGetValue<Options>(out _))
        {
            throw new InvalidOperationException("Options not set");
        }
    }

    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var options = (Options)bot.Properties[typeof(Options)];
        var timeout = options.GetRandomTimeout();
        using var cts1 = new CancellationTokenSource(timeout);
        using var cts2 = CancellationTokenSource.CreateLinkedTokenSource(
            cts1.Token, cancellationToken);
        while (!cts2.IsCancellationRequested)
        {
            var user = await bot.UpdateUserDataAsync(cts2.Token);
            if (user.SessionId != default)
            {
                bot.Properties[typeof(WaitSessionJob.Options)] = new WaitSessionJob.Options
                {
                    State = SessionState.Active,
                };
                bot.Properties[typeof(SubmitJob.Options)] = new SubmitJob.Options();
                break;
            }

            await Task.Delay(options.Interval, cts2.Token);
        }

        logger.LogInformation("User data updated");
    }

    protected override void OnFinished(IBot bot, Exception? exception)
    {
        if (exception is not null)
        {
            bot.Properties[typeof(CancelMatchingJob.Options)] = new CancelMatchingJob.Options();
        }

        bot.Properties.Remove(typeof(Options));
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
