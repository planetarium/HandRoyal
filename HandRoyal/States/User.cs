using Bencodex.Types;
using Libplanet.Crypto;

namespace HandRoyal.States;

public class User
{
    public User(Address id, List<Address> gloves)
    {
        Id = id;
        Gloves = gloves.ToList();
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

        Gloves = gloves.Select(v => new Address(v)).ToList();
    }

    public IValue Bencoded => List.Empty
        .Add(Id.Bencoded)
        .Add(new List(Gloves.Select(glove => glove.Bencoded)));

    public Address Id { get; }

    public List<Address> Gloves { get; }
}
