using GraphQL.AspNet.Interfaces.Subscriptions;
using GraphQL.AspNet.Schemas;
using Libplanet.Node.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandRoyal.Explorer.Publishers;

internal abstract class RenderActionEventPublisherBase<TEventData>(IServiceProvider serviceProvider)
    : IHostedService
{
    private readonly IRendererService _rendererService
        = serviceProvider.GetRequiredService<IRendererService>();

    private readonly ISubscriptionEventRouter _router
        = serviceProvider.GetRequiredService<ISubscriptionEventRouter>();

    private IDisposable? _observer;

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        _observer = _rendererService.RenderAction.Subscribe(OnRenderAction);
        await OnStartAsync(cancellationToken);
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        await OnStopAsync(cancellationToken);
        _observer?.Dispose();
        _observer = null;
    }

    protected virtual Task OnStartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task OnStopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected void RaisePublishedEvent(TEventData eventData, string eventName)
    {
        var subscriptionEvent = new GraphQL.AspNet.SubscriptionServer.SubscriptionEvent
        {
            Id = Guid.NewGuid().ToString(),
            EventName = eventName,
            Data = eventData,
            SchemaTypeName = typeof(GraphSchema).AssemblyQualifiedName,
            DataTypeName = typeof(TEventData).AssemblyQualifiedName,
        };
        _router.RaisePublishedEvent(subscriptionEvent);
    }

    protected abstract void OnRenderAction(RenderActionInfo info);
}
