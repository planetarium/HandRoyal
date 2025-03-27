using Bencodex.Types;
using HandRoyal.Actions;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class SessionEventPublisher(
    IServiceProvider serviceProvider,
    IActionService actionService,
    IStoreService storeService)
    : RenderActionEventPublisherBase<SessionEventData>(serviceProvider)
{
    private readonly Dictionary<Address, Session> _sessionById = [];

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
                    _sessionById[sessionId] = session;
                }
            }
            else if (name == "ProcessSession")
            {
                foreach (var session in sessions)
                {
                    var prevSession = _sessionById[session.Metadata.Id];
                    if (session != prevSession)
                    {
                        var eventData = new SessionEventData(session);
                        var eventName = SubscriptionController.SessionChangedEventName;
                        RaisePublishedEvent(eventData, eventName);
                    }
                }
            }
            else if (name == "PostProcessSession")
            {
                foreach (var session in sessions)
                {
                    if (!_sessionById.ContainsKey(session.Metadata.Id))
                    {
                        var eventData = new SessionEventData(session);
                        var eventName = SubscriptionController.SessionChangedEventName;
                        RaisePublishedEvent(eventData, eventName);
                    }
                }

                _sessionById.Clear();
            }
        }
        else
        {
            var typeId = GetTypeId(info.Action);
            var stateStore = storeService.StateStore;
            var trie = stateStore.GetStateRoot(info.NextState);
            var world = new WorldStateContext(trie, stateStore);
            if (typeId == "SubmitMove")
            {
                var submitMove = CreateAction<SubmitMove>(info.Action);
                var session = Session.GetSession(world, submitMove.SessionId);
                var eventData = new SessionEventData(session);
                RaisePublishedEvent(eventData, SubscriptionController.SessionChangedEventName);
            }
            else if (typeId == "CreateSession")
            {
                var createSession = CreateAction<CreateSession>(info.Action);
                var session = Session.GetSession(world, createSession.SessionId);
                var eventData = new SessionEventData(session);
                RaisePublishedEvent(eventData, SubscriptionController.SessionChangedEventName);
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
