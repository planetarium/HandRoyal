using Bencodex.Types;
using HandRoyal.Actions;
using HandRoyal.Explorer.Subscriptions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Publishers;

internal sealed class SessionEventPublisher(
    IServiceProvider serviceProvider, IStoreService storeService)
    : RenderActionEventPublisherBase<SessionEventData>(serviceProvider)
{
    protected override void OnRenderAction(RenderActionInfo info)
    {
        if (info.Action is List list)
        {
            if (list.Count == 1)
            {
                var name = (string)(Text)list[0];
                if (name == "ProcessSession")
                {
                    var stateStore = storeService.StateStore;
                    var trie = stateStore.GetStateRoot(info.NextState);
                    var world = new WorldBaseState(trie, stateStore);
                    var sessions = Session.GetSessions(world);
                    var height = info.Context.BlockIndex;
                    foreach (var session in sessions)
                    {
                        if (session.Height == height)
                        {
                            var eventData = new SessionEventData(session);
                            var eventName = SubscriptionController.SessionChangedEventName;
                            RaisePublishedEvent(eventData, eventName);
                        }
                    }
                }
            }
        }
        else
        {
            var typeId = GetTypeId(info.Action);
            var stateStore = storeService.StateStore;
            var trie = stateStore.GetStateRoot(info.NextState);
            var world = new WorldBaseState(trie, stateStore);
            if (typeId == "SubmitMove")
            {
                var submitMove = CreateAction<SubmitMove>(info.Action);
                var session = Session.FromState(world, submitMove.SessionId);
                var eventData = new SessionEventData(session);
                RaisePublishedEvent(eventData, SubscriptionController.SessionChangedEventName);
            }
            else if (typeId == "CreateSession")
            {
                var createSession = CreateAction<CreateSession>(info.Action);
                var session = Session.FromState(world, createSession.SessionId);
                var eventData = new SessionEventData(session);
                RaisePublishedEvent(eventData, SubscriptionController.SessionChangedEventName);
            }
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

    private static T CreateAction<T>(IValue value)
        where T : IAction
    {
        var action = Activator.CreateInstance<T>();
        action.LoadPlainValue(value);
        return action;
    }
}
