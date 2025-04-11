using HandRoyal.Bot.Jobs;

namespace HandRoyal.Bot;

public sealed class UserBot(BotOptions options)
    : BotBase(options)
{
    private bool _isCreated;

    protected override async Task<Type> SelectJobAsync(CancellationToken cancellationToken)
    {
        if (Properties.Contains(typeof(IdleJob.Options)))
        {
            return typeof(IdleJob);
        }

        if (Properties.Contains(typeof(UpdateUserJob.Options)))
        {
            return typeof(UpdateUserJob);
        }

        if (Properties.Contains(typeof(WaitMatchingJob.Options)))
        {
            return typeof(WaitMatchingJob);
        }

        if (Properties.Contains(typeof(CancelMatchingJob.Options)))
        {
            return typeof(CancelMatchingJob);
        }

        if (Properties.Contains(typeof(WaitSessionJob.Options)))
        {
            return typeof(WaitSessionJob);
        }

        if (Properties.Contains(typeof(SubmitMoveJob.Options)))
        {
            return typeof(SubmitMoveJob);
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
        else if (n < 80)
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
