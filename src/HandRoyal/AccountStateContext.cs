using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using HandRoyal.Serialization;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal;

public sealed class AccountStateContext(
    IAccountState account, Address address) : IAccountContext
{
    public Address Address { get; } = address;

    public bool IsReadOnly => true;

    public object this[Address address]
    {
        get
        {
            if (account.GetState(address) is not { } state)
            {
                throw new KeyNotFoundException($"No state found at {address}");
            }

            if (Serializer.TryGetType(state, out var type))
            {
                return Serializer.Deserialize(state, type)
                    ?? throw new InvalidOperationException("Failed to deserialize state.");
            }

            return state;
        }

        set => throw new NotSupportedException("Setting state is not supported.");
    }

    public bool TryGetObject<T>(Address address, [MaybeNullWhen(false)] out T value)
    {
        if (account.GetState(address) is { } state)
        {
            if (Serializer.TryGetType(state, out var type))
            {
                if (Serializer.Deserialize(state, type) is T obj)
                {
                    value = obj;
                    return true;
                }
            }
            else if (typeof(IBencodable).IsAssignableFrom(typeof(T)))
            {
                if (Activator.CreateInstance(typeof(T), args: [state]) is not T obj)
                {
                    throw new InvalidOperationException("Failed to create an instance of T.");
                }

                value = obj;
                return true;
            }
            else if (typeof(IValue).IsAssignableFrom(typeof(T)))
            {
                value = (T)state;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool TryGetState<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IValue
    {
        if (account.GetState(address) is T state)
        {
            value = state;
            return true;
        }

        value = default;
        return false;
    }

    public T GetState<T>(Address address, T fallback)
        where T : IValue
    {
        if (TryGetState<T>(address, out var value))
        {
            return value;
        }

        return fallback;
    }

    public bool Contains(Address address) => account.GetState(address) is not null;

    public bool Remove(Address address)
        => throw new NotSupportedException("Removing state is not supported.");
}
