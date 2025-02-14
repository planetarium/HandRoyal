using System.Diagnostics.CodeAnalysis;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Subscriptions;

internal sealed class SubscriptionController : GraphController
{
    public const string TipChangedEventName = "TIP_CHANGED";
    public const string MoveChangedEventName = "MOVE_CHANGED";
    public const string SessionChangedEventName = "SESSION_CHANGED";

    [SubscriptionRoot("onTipChanged", typeof(TipEventData), EventName = TipChangedEventName)]
    public IGraphActionResult OnTipChanged(TipEventData eventData)
    {
        return Ok(eventData);
    }

    [SubscriptionRoot(
        "onMoveChanged", typeof(SubmitMoveEventData), EventName = MoveChangedEventName)]
    public IGraphActionResult OnMoveChanged(
        SubmitMoveEventData eventData, Address sessionId, Address userId)
    {
        if (eventData.SessionId == sessionId && eventData.UserId == userId)
        {
            return Ok(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onSessionChanged", typeof(SessionEventData), EventName = SessionChangedEventName)]
    public IGraphActionResult OnSessionChanged(
        SessionEventData eventData, Address sessionId, Address userId)
    {
        var session = eventData.Session;
        if (session.Metadata.Id == sessionId)
        {
            if (TryGetMatch(session, userId, out var match))
            {
                eventData.Match = match;
            }

            return Ok(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    private static bool TryGetMatch(
        Session session, Address userId, [MaybeNullWhen(false)] out Match match)
    {
        var index = session.FindPlayer(userId);
        if (index >= 0 && session.Rounds.Length > 0)
        {
            var round = session.Rounds[^1];
            for (var i = 0; i < round.Matches.Length; i++)
            {
                match = round.Matches[i];
                if (match.Move1.PlayerIndex == index
                    || match.Move2.PlayerIndex == index)
                {
                    return true;
                }
            }
        }

        match = default;
        return false;
    }
}
