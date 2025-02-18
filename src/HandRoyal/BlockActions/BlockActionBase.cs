using System.Diagnostics;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.BlockActions;

public abstract class BlockActionBase : IAction
{
    IValue IAction.PlainValue => Null.Value;

    void IAction.LoadPlainValue(IValue plainValue)
        => throw new UnreachableException("This method should not be called.");

    IWorld IAction.Execute(IActionContext context) => OnExecute(context);

    protected abstract IWorld OnExecute(IActionContext context);
}
