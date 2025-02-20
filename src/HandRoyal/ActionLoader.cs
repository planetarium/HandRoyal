using System.Reflection;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.Sys;

namespace HandRoyal;

public sealed class ActionLoader : IActionLoader
{
    private static readonly Dictionary<IValue, Type> _typeById
        = LoadTypes(typeof(ActionLoader).Assembly);

    public IAction LoadAction(long index, IValue value)
    {
        try
        {
            if (Registry.IsSystemAction(value))
            {
                return Registry.Deserialize(value);
            }

            if (value is not List list)
            {
                throw new ArgumentException(
                    $"The given value is not a list: {value}",
                    nameof(value));
            }

            if (list[0] is not Text typeId)
            {
                throw new ArgumentException(
                    $"The first element of the list is not a text: {list[0]}",
                    nameof(value));
            }

            if (!_typeById.TryGetValue(typeId, out var actionType))
            {
                throw new ArgumentException(
                    $"No action type found for the given type ID: {typeId}",
                    nameof(value));
            }

            if (Activator.CreateInstance(actionType, list[1]) is not IAction action)
            {
                throw new InvalidOperationException(
                    $"Failed to instantiate an action from {value} for index {index}");
            }

            return action;
        }
        catch (Exception e)
        {
            throw new InvalidActionException(
                $"Failed to instantiate an action from {value} for index {index}",
                value,
                e);
        }
    }

    private static Dictionary<IValue, Type> LoadTypes(Assembly assembly)
    {
        var types = GetActionTypes(assembly);
        var capacity = types.Count();
        var typeById = new Dictionary<IValue, Type>(capacity);
        foreach (var type in types)
        {
            if (type.GetCustomAttribute<ActionTypeAttribute>() is not { } attribute)
            {
                continue;
            }

            if (type.GetConstructor([typeof(IValue)]) is null)
            {
                throw new InvalidOperationException(
                    $"The action type {type} does not have a constructor that takes an IValue.");
            }

            typeById.Add(attribute.TypeIdentifier, type);
        }

        return typeById;
    }

    private static IEnumerable<Type> GetActionTypes(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(type => typeof(IAction).IsAssignableFrom(type));
        foreach (var type in types)
        {
            yield return type;
        }
    }
}
