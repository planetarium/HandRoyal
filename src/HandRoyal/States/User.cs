using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.States;

public sealed record class User : IBencodable
{
    public User(Address id)
        : this(id, [])
    {
    }

    public User(Address id, ImmutableArray<Address> gloves)
    {
        Id = id;
        Gloves = gloves;
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

    public Address Id { get; }

    public ImmutableArray<Address> Gloves { get; set; }

    public Address SessionId { get; set; }

    public static User FromState(IWorldState worldState, Address userId)
    {
        var userAccount = worldState.GetAccountState(Addresses.Users);
        if (userAccount.GetState(userId) is not { } userState)
        {
            var message = $"User of id {userId} does not exist.";
            throw new ArgumentException(message, nameof(userId));
        }

        return new User(userState);
    }
}
