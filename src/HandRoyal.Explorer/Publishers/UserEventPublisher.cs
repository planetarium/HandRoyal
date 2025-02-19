using Bencodex.Types;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
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
        var world = new WorldBaseState(trie, stateStore);
        if (typeId == "CreateUser")
        {
            var user = User.FromState(world, info.Context.Signer);
            var eventData = new UserEventData(user);
            RaisePublishedEvent(eventData, SubscriptionController.UserChangedEventName);
        }
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
}
