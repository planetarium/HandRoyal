using Bencodex.Types;
using HandRoyal.Actions;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.Gloves;
using Libplanet.Action;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class PickUpEventPublisher(
    IServiceProvider serviceProvider)
    : RenderActionEventPublisherBase<PickUpResultEventData>(serviceProvider)
{
    protected override void OnRenderAction(RenderActionInfo info)
    {
        var typeId = GetTypeId(info.Action);
        if (typeId == "PickUp")
        {
            if (info.Context.TxId is not { } txIdNotNull)
            {
                // PickUp cannot be a blockAction
                return;
            }

            var random = info.Context.GetRandom();
            var glove = GloveLoader.PickUpGlove(random, false);
            var eventData = new PickUpResultEventData(txIdNotNull, [glove]);
            RaisePublishedEvent(eventData, SubscriptionController.PickUpResultEventName);
        }
        else if (typeId == "PickUpMany")
        {
            if (info.Context.TxId is not { } txIdNotNull)
            {
                // PickUp cannot be a blockAction
                return;
            }

            var random = info.Context.GetRandom();
            var pickups = Enumerable.Range(0, PickUpMany.Count - 1)
                .Select(_ => GloveLoader.PickUpGlove(random, false));
            pickups = pickups.Append(GloveLoader.PickUpGlove(random, true));
            var eventData = new PickUpResultEventData(txIdNotNull, pickups.ToArray());
            RaisePublishedEvent(eventData, SubscriptionController.PickUpResultEventName);
        }
    }

    private static string GetTypeId(IValue value)
    {
        if (value is List list && list.Count == 2 && list[0] is Text typeIdText)
        {
            return typeIdText.Value;
        }

        return string.Empty;
    }
}
