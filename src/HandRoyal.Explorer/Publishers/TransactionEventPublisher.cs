using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class TransactionEventPublisher(IServiceProvider serviceProvider)
    : RenderBlockEventPublisherBase<TransactionEventData>(serviceProvider)
{
    protected override void OnRenderBlock(RenderBlockInfo info)
    {
        var block = info.NewTip;
        var eventData = new TransactionEventData
        {
            TxIds = [.. block.Transactions.Select(item => item.Id)],
            BlockHash = block.Hash,
            BlockHeight = block.Index,
        };
        RaisePublishedEvent(eventData, SubscriptionController.TransactionChangedEventName);
    }
}
