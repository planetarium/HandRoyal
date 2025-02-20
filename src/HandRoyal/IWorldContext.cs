using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal;

public interface IWorldContext
{
    bool IsReadOnly { get; }

    IAccountContext this[Address address] { get; }

    FungibleAssetValue GetBalance(Address address, Currency currency);

    void TransferAsset(Address sender, Address recipient, FungibleAssetValue value);
}
