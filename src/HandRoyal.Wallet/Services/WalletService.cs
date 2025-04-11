using System.Security.Cryptography;
using HandRoyal.Wallet.Interfaces;
using HandRoyal.Wallet.Models;
using Libplanet.Crypto;

namespace HandRoyal.Wallet.Services;

public class WalletService : IWalletService
{
    private readonly Dictionary<string, UserWallet> _wallets = new();
    private readonly ICryptoTransform _encryptor;
    private readonly ICryptoTransform _decryptor;

    public WalletService()
    {
        using var aes = Aes.Create();
        if (aes == null)
        {
            throw new InvalidOperationException("Failed to create AES instance.");
        }

        aes.GenerateKey();
        aes.GenerateIV();
        _encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        _decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    }

    public WalletService(byte[] key, byte[] iv)
    {
        _encryptor = Aes.Create().CreateEncryptor(key, iv);
        _decryptor = Aes.Create().CreateDecryptor(key, iv);
    }

    public async Task<byte[]> Sign(string userId, byte[] payload)
    {
        if (!_wallets.TryGetValue(userId, out var wallet))
        {
            throw new KeyNotFoundException($"Wallet not found for user {userId}");
        }

        var privateKey = PrivateKeyHandler.Decrypt(wallet.EncryptedPrivateKey, _decryptor);
        return await Task.FromResult(privateKey.Sign(payload));
    }

    public async Task<Address> GetAddressAsync(string userId)
    {
        if (!_wallets.TryGetValue(userId, out var wallet))
        {
            throw new KeyNotFoundException($"Wallet not found for user {userId}");
        }

        return await Task.FromResult(
            PrivateKeyHandler.Decrypt(wallet.EncryptedPrivateKey, _decryptor).Address);
    }

    public async Task<Address> CreateWalletAsync(string userId)
    {
        if (_wallets.ContainsKey(userId))
        {
            throw new InvalidOperationException($"Wallet already exists for user {userId}");
        }

        var privateKey = new PrivateKey();
        var encryptedPrivateKey = PrivateKeyHandler.Encrypt(privateKey, _encryptor);

        var wallet = new UserWallet
        {
            UserId = userId,
            EncryptedPrivateKey = encryptedPrivateKey,
        };

        _wallets[userId] = wallet;

        return await Task.FromResult(privateKey.Address);
    }
}
