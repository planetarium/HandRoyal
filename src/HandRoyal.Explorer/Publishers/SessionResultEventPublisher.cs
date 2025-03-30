using Bencodex.Types;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class SessionResultEventPublisher(
    IServiceProvider serviceProvider,
    IStoreService storeService)
    : RenderActionEventPublisherBase<SessionResultEventData>(serviceProvider)
{
    private readonly List<Address> _sessionIds = [];

    protected override void OnRenderAction(RenderActionInfo info)
    {
        if (info.Action is Text typeIdText)
        {
            var name = typeIdText.Value;
            var stateStore = storeService.StateStore;
            var trie = stateStore.GetStateRoot(info.NextState);
            var world = new WorldStateContext(trie, stateStore);
            var sessions = Session.GetSessions(world);
            if (name == "PreProcessSession")
            {
                foreach (var session in sessions)
                {
                    var sessionId = session.Metadata.Id;
                    _sessionIds.Add(sessionId);
                }
            }
            else if (name == "PostProcessSession")
            {
                foreach (var sessionId in _sessionIds)
                {
                    var session = Session.GetSession(world, sessionId);
                    if (!sessions.Select(s => s.Metadata.Id).Contains(session.Metadata.Id))
                    {
                        var eventData = new SessionResultEventData(session);
                        var eventName = SubscriptionController.SessionResultChangedEventName;
                        RaisePublishedEvent(eventData, eventName);
                    }
                }

                _sessionIds.Clear();
            }
        }
    }
}
