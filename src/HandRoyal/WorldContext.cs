using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal;

public sealed class WorldContext(IWorld world) : IDisposable
{
    private readonly Dictionary<Address, AccountContext> _accountByAddress = [];
    private IWorld _world = world;
    private bool _disposed;

    public IWorld World => _world;

    public AccountContext this[Address accountAddress]
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!_accountByAddress.TryGetValue(accountAddress, out var accountContext))
            {
                var account = World.GetAccount(accountAddress);
                var setter = new Action<IAccount>(item => SetAccount(accountAddress, item));

                accountContext = new AccountContext(account, setter);
                _accountByAddress[accountAddress] = accountContext;
            }

            return accountContext;
        }
    }

    void IDisposable.Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    private void SetAccount(Address accountAddress, IAccount account)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _world = _world.SetAccount(accountAddress, account);
    }
}
