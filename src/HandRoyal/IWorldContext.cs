using System.Diagnostics.CodeAnalysis;
using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal;

public interface IWorldContext
{
    bool IsReadOnly { get; }

    IAccountContext this[Address address] { get; }

    object this[Address address, Address stateAddress]
    {
        get => this[address][stateAddress];
        set => this[address][stateAddress] = value;
    }

    FungibleAssetValue GetBalance(Address address, Currency currency);

    void TransferAsset(Address sender, Address recipient, FungibleAssetValue value);

    bool TryGetObject<T>(Address address, Address stateAddress, [MaybeNullWhen(false)] out T value)
    {
        if (this[address].TryGetObject<T>(stateAddress, out var obj))
        {
            value = obj;
            return true;
        }

        value = default;
        return false;
    }

    bool Contains(Address address, Address stateAddress)
        => this[address].Contains(stateAddress);

    bool Remove(Address address, Address stateAddress)
        => this[address].Remove(stateAddress);
}
