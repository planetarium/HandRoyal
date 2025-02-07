using Libplanet.Action;
using Libplanet.Action.State;

namespace LastHandStanding.BlockActions;

internal sealed class SessionAction : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        return context.PreviousState;
    }
}
