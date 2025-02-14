using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using HandRoyal.Explorer.Types;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Subscriptions;

internal sealed class SubscriptionController : GraphController
{
    public const string TipChangedEventName = "TIP_CHANGED";
    public const string MoveChangedEventName = "SESSION_CHANGED";

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
}
