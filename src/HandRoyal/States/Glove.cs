using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.States;

public sealed record class Glove : IBencodable
{
    public Glove(Address id, Address author)
    {
        Id = id;
        Author = author;
    }

    public Glove(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.", nameof(value));
        }

        Id = ToAddress(list, 0);
        Author = ToAddress(list, 1);
    }

    public IValue Bencoded => new List(
        ToValue(Id),
        ToValue(Author));

    public Address Id { get; }

    public Address Author { get; }
}
