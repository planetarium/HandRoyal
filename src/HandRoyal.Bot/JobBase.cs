namespace HandRoyal.Bot;

public abstract class JobBase(string name) : IJob
{
    public string Name { get; } = name;

    async Task IJob.ExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        Verify(bot);
        try
        {
            await OnExecuteAsync(bot, cancellationToken);
        }
        catch (Exception e)
        {
            OnFinished(bot, e);
            throw;
        }
        finally
        {
            OnFinished(bot, null);
        }
    }

    protected abstract Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken);

    protected void UpdateState(IBot bot, string state)
    {
        if (bot is IJobObserver jobObserver)
        {
            jobObserver.OnJobUpdated(GetType(), Name, state);
        }
    }

    protected virtual void Verify(IBot bot)
    {
    }

    protected virtual void OnFinished(IBot bot, Exception? exception)
    {
    }
}
