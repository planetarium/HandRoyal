using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandRoyal.Bot;

internal sealed class JobService(IServiceProvider serviceProvider)
    : IJobService, IHostedService
{
    private JobCollection? _states;

    public JobCollection Jobs => _states
        ?? throw new InvalidOperationException("States have not been initialized.");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _states = new([.. serviceProvider.GetServices<IJob>()]);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _states = null;
        return Task.CompletedTask;
    }
}
