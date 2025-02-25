using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class User : IEquatable<User>
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public ImmutableArray<Address> Gloves { get; init; } = [];

    [Property(2)]
    public Address SessionId { get; init; }

    public static User GetUser(IWorldContext world, Address userId)
        => world[Addresses.Users, userId] is User user
            ? user : throw new KeyNotFoundException("User not found.");

    public static bool TryGetUser(
        IWorldContext world, Address userId, [MaybeNullWhen(false)] out User user)
    {
        var usersAccount = world[Addresses.Users];
        return usersAccount.TryGetValue(userId, out user);
    }

    public bool Equals(User? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
