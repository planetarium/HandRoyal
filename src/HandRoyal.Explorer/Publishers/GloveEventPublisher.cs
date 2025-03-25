using Bencodex.Types;
using HandRoyal.Actions;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.Gloves;
using Libplanet.Action;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class GloveEventPublisher(
    IServiceProvider serviceProvider,
    IActionService actionService,
    IStoreService storeService)
    : RenderActionEventPublisherBase<GloveRegisteredEventData>(serviceProvider)
{
    protected override void OnRenderAction(RenderActionInfo info)
    {
        var typeId = GetTypeId(info.Action);
        if (typeId == "RegisterGlove")
        {
            var registerGlove = CreateAction<RegisterGlove>(info.Action);
            var stateStore = storeService.StateStore;
            var trie = stateStore.GetStateRoot(info.NextState);
            var world = new WorldStateContext(trie, stateStore);
            var glovesAccount = world[Addresses.Gloves];
            var gloveState = glovesAccount.GetValue<IGlove?>(registerGlove.Id, null);
            if (gloveState != null)
            {
                var eventData = new GloveRegisteredEventData(gloveState);
                RaisePublishedEvent(eventData, SubscriptionController.GloveRegisteredEventName);
            }
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

    private T CreateAction<T>(IValue value)
        where T : IAction
    {
        var actionLoader = actionService.ActionLoader;
        return (T)actionLoader.LoadAction(0, value);
    }
}
