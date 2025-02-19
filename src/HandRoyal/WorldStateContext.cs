using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Types.Assets;

namespace HandRoyal;

public sealed class WorldStateContext(ITrie trie, IStateStore stateStore) : IWorldContext
{
    private readonly Dictionary<Address, AccountStateContext> _accountByAddress = [];
    private readonly WorldBaseState _world = new(trie, stateStore);

    public bool IsReadOnly => true;

    public AccountStateContext this[Address address]
    {
        get
        {
            if (!_accountByAddress.TryGetValue(address, out var accountContext))
            {
                var account = _world.GetAccountState(address);
                accountContext = new AccountStateContext(account, address);
                _accountByAddress[address] = accountContext;
            }

            return accountContext;
        }
    }

    IAccountContext IWorldContext.this[Address address] => this[address];

    public FungibleAssetValue GetBalance(Address address, Currency currency)
        => _world.GetBalance(address, currency);

    public void TransferAsset(Address sender, Address recipient, FungibleAssetValue value)
        => throw new NotSupportedException("This method is not supported in this context.");
}
