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

    public bool IsReadOnly => false;

    public object this[Address address]
    {
        get => _account.GetState(address)
            ?? throw new KeyNotFoundException($"No state found at {address}");
        set
        {
            if (value is IValue state)
            {
                _account = _account.SetState(address, state);
                setter(this);
            }
            else if (value is IBencodable obj)
            {
                if (obj is IValidateState validateState)
                {
                    validateState.Validate();
                }

                _account = _account.SetState(address, obj.Bencoded);
                setter(this);
            }
            else
            {
                throw new NotSupportedException("Setting state is not supported.");
            }
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

    public bool ContainsState(Address address) => _account.GetState(address) is not null;

    public bool RemoveState(Address address)
    {
        if (_account.GetState(address) is not null)
        {
            _account = _account.RemoveState(address);
            setter(this);
            return true;
        }

        return false;
    }
}
