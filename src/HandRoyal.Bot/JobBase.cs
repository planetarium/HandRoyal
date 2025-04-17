using System.Text.RegularExpressions;

namespace HandRoyal.Bot;

public abstract class JobBase : IJob
{
    protected JobBase(string name) => Name = name;

    protected JobBase()
    {
        Name = Regex.Replace(GetType().Name, "(.+)Job", "$1", RegexOptions.IgnoreCase);
    }

    public string Name { get; }

    async Task IJob.ExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var context = new JobContext(bot);
        Verify(context);
        try
        {
            StartJob(bot);
            await OnExecuteAsync(context, cancellationToken);
            OnFinished(context, null);
            FinishJob(bot, null);
        }
        catch (Exception e)
        {
            OnFinished(context, e);
            FinishJob(bot, e);
            throw;
        }
    }

    protected abstract Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken);

    protected void UpdateState(IJobContext context, string state)
    {
        if (context is JobContext jobContext)
        {
            jobContext.OnJobUpdated(this, state);
        }
    }

    protected virtual void Verify(IJobContext context)
    {
    }

    protected virtual void OnFinished(IJobContext context, Exception? exception)
    {
    }

    private void StartJob(IBot bot)
    {
        if (bot is IJobObserver jobObserver)
        {
            jobObserver.OnStarted(GetType(), Name);
        }
    }

    private void FinishJob(IBot bot, Exception? exception)
    {
        if (bot is IJobObserver jobObserver)
        {
            jobObserver.OnFinished(GetType(), Name, exception);
        }
    }
}
