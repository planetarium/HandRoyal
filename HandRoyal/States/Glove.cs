using Bencodex.Types;
using Libplanet.Crypto;

namespace HandRoyal.States;

public sealed record class Glove
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
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Id = new Address(list[0]);
        Author = new Address(list[1]);
    }

    public IValue Bencoded => new List(
        Id.Bencoded,
        Author.Bencoded);

    public Address Id { get; }

    public Address Author { get; }
}
