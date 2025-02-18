using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class TipEventPublisher(IServiceProvider serviceProvider)
    : RenderBlockEventPublisherBase<TipEventData>(serviceProvider)
{
    protected override void OnRenderBlock(RenderBlockInfo info)
    {
        var tip = info.NewTip;
        var eventData = new TipEventData
        {
            Height = tip.Index,
            Hash = tip.Hash,
        };

        RaisePublishedEvent(eventData, SubscriptionController.TipChangedEventName);
    }
}
