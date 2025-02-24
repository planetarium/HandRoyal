using System.Diagnostics;
using System.Reflection;
using Bencodex.Types;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.Actions;

public abstract record class ActionBase : IAction
{
    protected ActionBase()
    {
    }

    IValue IAction.PlainValue => new List(
        TypeId,
        Serializer.Serialize(this));

    private IValue TypeId =>
        GetType().GetCustomAttribute<ActionTypeAttribute>() is { } attribute
            ? attribute.TypeIdentifier
            : throw new InvalidOperationException(
                $"Type is missing {nameof(ActionTypeAttribute)}: {GetType()}");

    void IAction.LoadPlainValue(IValue plainValue)
        => throw new UnreachableException("This method should not be called.");

    IWorld IAction.Execute(IActionContext context)
    {
        using var worldContext = new WorldContext(context);
        OnExecute(worldContext, context);
        return worldContext.Flush();
    }

    protected abstract void OnExecute(IWorldContext world, IActionContext context);
}
