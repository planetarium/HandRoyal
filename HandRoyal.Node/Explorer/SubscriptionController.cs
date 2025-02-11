using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using HandRoyal.Node.Explorer.Types;

namespace HandRoyal.Node.Explorer;

public sealed class SubscriptionController : GraphController
{
    public const string TipChangedEventName = "TIP_CHANGED";

    [SubscriptionRoot("onTipChanged", typeof(TipEventData), EventName = TipChangedEventName)]
    public IGraphActionResult OnTipChanged(TipEventData eventData)
    {
        return Ok(eventData);
    }
}
