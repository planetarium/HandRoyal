using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal;

internal sealed class AccountContext(
    IAccount account, Address address, Action<AccountContext> setter) : IAccountContext
{
    private IAccount _account = account;

    public Address Address { get; } = address;

    public IAccount Account => _account;

    public IValue this[Address address]
    {
        get => _account.GetState(address) ?? throw new KeyNotFoundException(
            $"No state found at {address}");
        set
        {
            _account = _account.SetState(address, value);
            setter(this);
        }
    }

    public bool TryGetObject<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IBencodable
    {
        if (_account.GetState(address) is { } state
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
        if (_account.GetState(address) is T state)
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

    public bool Contains(Address address) => _account.GetState(address) is not null;

    public bool Remove(Address address)
    {
        _account.RemoveState(address);
        return true;
    }
}
