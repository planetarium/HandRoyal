using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

namespace LastHandStanding.States;

public sealed record class SessionMetadata : IBencodable
{
    public SessionMetadata(Address id, Address organizer, Address prize)
    {
        Id = id;
        Organizer = organizer;
        Prize = prize;
    }

    public SessionMetadata(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Id = new Address(list[0]);
        Organizer = new Address(list[1]);
        Prize = new Address(list[2]);
        MaximumUser = (int)(Integer)list[4];
        MinimumUser = (int)(Integer)list[5];
        RemainingUser = (int)(Integer)list[6];
    }

    public IValue Bencoded => new List(
        Id.Bencoded,
        Organizer.Bencoded,
        Prize.Bencoded,
        (Integer)MaximumUser,
        (Integer)MinimumUser,
        (Integer)RemainingUser);

    public Address Id { get; }

    public Address Organizer { get; }

    public Address Prize { get; }

    public int MaximumUser { get; set; } = 100;

    public int MinimumUser { get; set; } = 10;

    public int RemainingUser { get; set; } = 10;

    public long RoundInterval { get; set; } = 10;

    public long WaitingInterval { get; set; } = 10;
}
