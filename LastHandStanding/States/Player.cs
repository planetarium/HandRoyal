using Bencodex.Types;
using Libplanet.Crypto;

namespace LastHandStanding.States;

public class Player
{
    public Player(Address id, Address glove)
    {
        Id = id;
        Glove = glove;
        State = 0;
    }

    public Player(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Id = new Address(list[0]);
        Glove = new Address(list[1]);
        State = (Integer)list[2];
    }

    public IValue Bencoded => List.Empty
        .Add(Id.Bencoded)
        .Add(Glove.Bencoded)
        .Add(State);
    
    public Address Id { get; }
    
    public Address Glove { get; }

    public int State { get; set; }
}