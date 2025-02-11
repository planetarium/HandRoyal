using GraphQL.AspNet.Interfaces.Subscriptions;
using GraphQL.AspNet.Schemas;
using HandRoyal.Node.Explorer.Types;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer.Publishers;

internal sealed class BlockChainRendererEventPublisher(
    IRendererService rendererService,
    ISubscriptionEventRouter router)
    : IHostedService
{
    private IDisposable? _observer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _observer = rendererService.RenderBlockEnd.Subscribe(RenderBlockEnd);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _observer?.Dispose();
        _observer = null;
        return Task.CompletedTask;
    }

    private void RenderBlockEnd(RenderBlockInfo info)
    {
        var eventData = new TipEventData { Height = info.NewTip.Index };
        var subscriptionEvent = new GraphQL.AspNet.SubscriptionServer.SubscriptionEvent
        {
            Id = Guid.NewGuid().ToString(),
            EventName = SubscriptionController.TipChangedEventName,
            Data = eventData,
            SchemaTypeName = typeof(GraphSchema).AssemblyQualifiedName,
            DataTypeName = typeof(TipEventData).AssemblyQualifiedName,
        };
        router.RaisePublishedEvent(subscriptionEvent);
    }
}
