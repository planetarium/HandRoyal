using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

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

        Id = new Address(list[0]);

        if (list[1] is not List gloves)
        {
            throw new ArgumentException($"Given value {value} is not a list", nameof(value));
        }

        Gloves = [.. gloves.Select(v => new Address(v))];
    }

    public IValue Bencoded => new List(
        Id.Bencoded,
        new List(Gloves.Select(glove => glove.Bencoded)));

    public Address Id { get; }

    public ImmutableArray<Address> Gloves { get; set; }
}
