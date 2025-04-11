using System.Collections.Immutable;
using System.Security.Cryptography;
using Libplanet.Crypto;

namespace HandRoyal.Wallet.Models
{
    public static class PrivateKeyHandler
    {
        public static EncryptedPrivateKey Encrypt(
            PrivateKey privateKey,
            ICryptoTransform encryptor)
        {
            var decrypted = privateKey.ToByteArray();
            var encrypted = encryptor.TransformFinalBlock(decrypted, 0, decrypted.Length);

            return new EncryptedPrivateKey { ByteArray = encrypted.ToImmutableArray() };
        }

        public static PrivateKey Decrypt(
            EncryptedPrivateKey encryptedPrivateKey,
            ICryptoTransform decryptor)
        {
            var encrypted = encryptedPrivateKey.ToByteArray();
            var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return new PrivateKey(decrypted);
        }
    }
}
