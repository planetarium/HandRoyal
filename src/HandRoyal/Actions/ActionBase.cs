using System.Reflection;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.Actions;

public abstract class ActionBase : IAction
{
    public IValue PlainValue =>
        Dictionary.Empty
            .Add("type_id", TypeId)
            .Add("values", PlainValueInternal);

    protected abstract IValue PlainValueInternal { get; }

    private IValue TypeId =>
        GetType().GetCustomAttribute<ActionTypeAttribute>() is { } attribute
            ? attribute.TypeIdentifier
            : throw new InvalidOperationException(
                $"Type is missing {nameof(ActionTypeAttribute)}: {GetType()}");

    public void LoadPlainValue(IValue plainValue)
    {
        if (plainValue is not Dictionary dict)
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} must be a {nameof(Dictionary)}: " +
                $"{plainValue.GetType()}",
                nameof(plainValue));
        }

        if (!dict.TryGetValue((Text)"type_id", out IValue typeId))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} is missing type id: {plainValue}",
                nameof(plainValue));
        }

        if (!typeId.Equals(TypeId))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} has invalid type id: {plainValue}",
                nameof(plainValue));
        }

        if (!dict.TryGetValue((Text)"values", out IValue values))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} is missing values: {plainValue}",
                nameof(plainValue));
        }

        LoadPlainValueInternal(values);
    }

    public abstract IWorld Execute(IActionContext context);

    protected abstract void LoadPlainValueInternal(IValue plainValueInternal);
}
