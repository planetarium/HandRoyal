using Bencodex.Types;
using HandRoyal.Enums;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class MatchMadeEventPublisher(
    IServiceProvider serviceProvider,
    IStoreService storeService)
    : RenderActionEventPublisherBase<MatchMadeEventData>(serviceProvider)
{
    protected override void OnRenderAction(RenderActionInfo info)
    {
        if (info.Action is not Text typeIdText)
        {
            return;
        }

        var name = typeIdText.Value;
        if (name != "ProcessMatching")
        {
            return;
        }

        var stateStore = storeService.StateStore;
        var trie = stateStore.GetStateRoot(info.NextState);
        var world = new WorldStateContext(trie, stateStore);
        var sessions = Session.GetSessions(world);

        foreach (var session in sessions)
        {
            if (session.State != SessionState.None ||
                session.State != SessionState.Ready)
            {
                continue;
            }

            var eventData = new MatchMadeEventData(
                session.Metadata.Id,
                session.Players.Select(player => player.Id).ToArray());
            var eventName = SubscriptionController.MatchMadeEventName;
            RaisePublishedEvent(eventData, eventName);
        }
    }
}
