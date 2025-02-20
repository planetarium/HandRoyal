using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
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
        get => account.GetState(address)
            ?? throw new KeyNotFoundException($"No state found at {address}");
        set => throw new NotSupportedException("Setting state is not supported.");
    }

    public bool TryGetObject<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IBencodable
    {
        if (account.GetState(address) is { } state
            && Activator.CreateInstance(typeof(T), args: [state]) is T obj)
        {
            value = obj;
            return true;
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

    public bool ContainsState(Address address) => account.GetState(address) is not null;

    public bool RemoveState(Address address)
        => throw new NotSupportedException("Removing state is not supported.");
}
