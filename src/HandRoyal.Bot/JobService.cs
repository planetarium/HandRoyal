using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandRoyal.Bot;

internal sealed class JobService(IServiceProvider serviceProvider, IBotService botService)
    : IJobService, IHostedService
{
    private JobCollection? _states;

    public JobCollection Jobs => _states ?? JobCollection.Empty;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (botService.IsEnabled)
        {
            _states = new([.. serviceProvider.GetServices<IJob>()]);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _states = null;
        return Task.CompletedTask;
    }
}
