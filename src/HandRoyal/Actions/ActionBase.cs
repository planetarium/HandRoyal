using System.Diagnostics;
using System.Reflection;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.Actions;

public abstract record class ActionBase : IAction
{
    protected ActionBase()
    {
    }

    protected ActionBase(IValue value)
    {
    }

    IValue IAction.PlainValue => new List(
        TypeId,
        PlainValue);

    protected abstract IValue PlainValue { get; }

    private IValue TypeId =>
        GetType().GetCustomAttribute<ActionTypeAttribute>() is { } attribute
            ? attribute.TypeIdentifier
            : throw new InvalidOperationException(
                $"Type is missing {nameof(ActionTypeAttribute)}: {GetType()}");

    void IAction.LoadPlainValue(IValue plainValue)
        => throw new UnreachableException("This method should not be called.");

    IWorld IAction.Execute(IActionContext context)
    {
        using var worldContext = new WorldContext(context.PreviousState);
        OnExecute(worldContext, context);
        return worldContext.World;
    }

    protected abstract void OnExecute(WorldContext world, IActionContext context);
}
