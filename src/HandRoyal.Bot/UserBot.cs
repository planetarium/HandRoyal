using HandRoyal.Bot.Jobs;

namespace HandRoyal.Bot;

public sealed class UserBot(BotOptions options)
    : BotBase(options)
{
    private static readonly SortedDictionary<Type, Type> _jobByOptions = new()
    {
        { typeof(IdleJob.Options), typeof(IdleJob) },
        { typeof(UpdateUserJob.Options), typeof(UpdateUserJob) },
        { typeof(WaitMatchingJob.Options), typeof(WaitMatchingJob) },
        { typeof(CancelMatchingJob.Options), typeof(CancelMatchingJob) },
        { typeof(WaitSessionJob.Options), typeof(WaitSessionJob) },
        { typeof(SubmitJob.Options), typeof(SubmitJob) },
    };

    private bool _isCreated;

    protected override async Task<Type> SelectJobAsync(CancellationToken cancellationToken)
    {
        foreach (var (option, job) in _jobByOptions)
        {
            if (Properties.Contains(option))
            {
                return job;
            }
        }

        if (!_isCreated)
        {
            return typeof(CreateUserJob);
        }

        var n = Random.Shared.Next(0, 100);

        if (n < 5)
        {
            return typeof(PickUpJob);
        }
        else if (n < 10)
        {
            return typeof(PickUpManyJob);
        }
        else if (n < 20)
        {
            Properties[typeof(RegisterMatchingJob.Options)] = new RegisterMatchingJob.Options();
            return typeof(RegisterMatchingJob);
        }

        await Task.CompletedTask;
        return typeof(IdleJob);
    }

    protected override void OnJobFinished(JobFinishedEventArgs e)
    {
        if (e.Type == typeof(CreateUserJob) && e.Exception is null)
        {
            _isCreated = true;
        }

        base.OnJobFinished(e);
    }
}
