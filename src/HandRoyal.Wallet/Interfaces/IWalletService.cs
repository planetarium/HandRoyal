using Libplanet.Crypto;

namespace HandRoyal.Wallet.Interfaces;

public interface IWalletService
{
    Task<byte[]> Sign(string userId, byte[] payload);

    Task<Address> GetAddressAsync(string userId);

    Task<Address> CreateWalletAsync(string userId);
}
