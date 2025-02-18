using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal;

public sealed class AccountContext(IAccount account, Action<IAccount> setter)
{
    private IAccount _account = account;

    public IValue this[Address stateAddress]
    {
        get => _account.GetState(stateAddress) ?? throw new KeyNotFoundException(
            $"No state found at {stateAddress}");
        set
        {
            _account = _account.SetState(stateAddress, value);
            setter(_account);
        }
    }

    public bool TryGetObject<T>(Address stateAddress, [MaybeNullWhen(false)] out T value)
        where T : IBencodable
    {
        if (_account.GetState(stateAddress) is { } state
            && Activator.CreateInstance(typeof(T), args: [state]) is T obj)
        {
            value = obj;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetState<T>(Address stateAddress, [MaybeNullWhen(false)] out T value)
        where T : IValue
    {
        if (_account.GetState(stateAddress) is T state)
        {
            value = state;
            return true;
        }

        value = default;
        return false;
    }

    public T GetState<T>(Address stateAddress, T fallback)
        where T : IValue
    {
        if (TryGetState<T>(stateAddress, out var value))
        {
            return value;
        }

        return fallback;
    }

    public bool Contains(Address stateAddress) => _account.GetState(stateAddress) is not null;
}
