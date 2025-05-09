using Bencodex.Types;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class UserEventPublisher(
    IServiceProvider serviceProvider, IStoreService storeService)
    : RenderActionEventPublisherBase<UserEventData>(serviceProvider)
{
    protected override void OnRenderAction(RenderActionInfo info)
    {
        var typeId = GetTypeId(info.Action);
        var stateStore = storeService.StateStore;
        var trie = stateStore.GetStateRoot(info.NextState);
        var world = new WorldStateContext(trie, stateStore);
        if (typeId == "CreateUser" || typeId == "JoinSession")
        {
            var user = User.GetUser(world, info.Context.Signer);
            var eventData = new UserEventData(user);
            RaisePublishedEvent(eventData, SubscriptionController.UserChangedEventName);
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
