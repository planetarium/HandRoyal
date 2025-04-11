namespace HandRoyal.Wallet.Models;

public record class UserWallet
{
    public required string UserId { get; init; }

    public required EncryptedPrivateKey EncryptedPrivateKey { get; init; }
}
