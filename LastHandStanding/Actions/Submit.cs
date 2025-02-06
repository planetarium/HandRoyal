using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

namespace LastHandStanding.Actions;

[ActionType("Submit")]
public class Submit : ActionBase
{
    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        throw new NotImplementedException();
    }

    public override IWorld Execute(IActionContext context)
    {
        throw new NotImplementedException();
    }

    protected override IValue PlainValueInternal { get; }
}