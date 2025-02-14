using Bencodex.Types;
using GraphQL.AspNet.Interfaces.Subscriptions;
using GraphQL.AspNet.Schemas;
using HandRoyal.Actions;
using HandRoyal.Node.Explorer.Subscriptions;
using HandRoyal.Node.Explorer.Types;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer.Publishers;

internal sealed class SubmitMoveRendererEventPublisher(
    IRendererService rendererService,
    ISubscriptionEventRouter router)
    : IHostedService
{
    private IDisposable? _observer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        rendererService.RenderAction.Subscribe(RenderAction);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _observer?.Dispose();
        _observer = null;
        return Task.CompletedTask;
    }

    private static string GetTypeId(IValue value)
    {
        if (value is not Dictionary dictionary)
        {
            return string.Empty;
        }

        if (!dictionary.TryGetValue((Text)"type_id", out var typeIdValue))
        {
            return string.Empty;
        }

        if (typeIdValue is not Text typeIdText)
        {
            return string.Empty;
        }

        return typeIdText;
    }

    private void RenderAction(RenderActionInfo info)
    {
        var typeId = GetTypeId(info.Action);

        if (typeId == "SubmitMove")
        {
            var submitMove = new SubmitMove();
            var signer = info.Context.Signer;
            submitMove.LoadPlainValue(info.Action);

            var eventData = new SubmitMoveEventData
            {
                SessionId = submitMove.SessionId,
                Move = submitMove.Move,
                UserId = signer,
            };
            var subscriptionEvent = new GraphQL.AspNet.SubscriptionServer.SubscriptionEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventName = SubscriptionController.MoveChangedEventName,
                Data = eventData,
                SchemaTypeName = typeof(GraphSchema).AssemblyQualifiedName,
                DataTypeName = typeof(SubmitMoveEventData).AssemblyQualifiedName,
            };
            router.RaisePublishedEvent(subscriptionEvent);
        }
    }
}
