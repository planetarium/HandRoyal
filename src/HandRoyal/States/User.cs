using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.States;

public sealed record class User : IBencodable
{
    public User()
    {
    }

    public User(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list", nameof(value));
        }

        Id = ToAddress(list, 0);
        Gloves = ToAddresses(list, 1);
        SessionId = ToAddress(list, 2);
    }

    IValue IBencodable.Bencoded => new List(
        ToValue(Id),
        ToValue(Gloves),
        ToValue(SessionId));

    public required Address Id { get; init; }

    public ImmutableArray<Address> Gloves { get; init; } = [];

    public Address SessionId { get; init; }

    public static User FromState(IWorldContext world, Address userId)
    {
        var usersAccount = world[Addresses.Users];
        if (!usersAccount.TryGetObject<User>(userId, out var user))
        {
            var message = $"User of id {userId} does not exist.";
            throw new ArgumentException(message, nameof(userId));
        }

        return user;
    }

    public static bool TryGetUser(
        IWorldContext world, Address userId, [MaybeNullWhen(false)] out User user)
    {
        var usersAccount = world[Addresses.Users];
        return usersAccount.TryGetObject(userId, out user);
    }
}
