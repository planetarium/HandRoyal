using System.Collections.Immutable;

namespace HandRoyal.Wallet.Models
{
    public record class EncryptedPrivateKey
    {
        public required ImmutableArray<byte> ByteArray { get; init; }

        public byte[] ToByteArray() => ByteArray.ToArray();
    }
}
