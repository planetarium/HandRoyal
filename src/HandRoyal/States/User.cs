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
    public Address EquippedGlove { get; init; }

    [Property(3)]
    public Address SessionId { get; init; }

    public static User GetUser(IWorldContext world, Address userId)
        => (User)world[Addresses.Users, userId];

    public static bool TryGetUser(
        IWorldContext world, Address userId, [MaybeNullWhen(false)] out User user)
        => world.TryGetValue(Addresses.Users, userId, out user);

    public bool Equals(User? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
