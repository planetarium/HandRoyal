using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.States;

public sealed record class SessionMetadata : IBencodable
{
    public SessionMetadata(
        Address id,
        Address organizer,
        Address prize,
        int maximumUser = 8,
        int minimumUser = 2,
        int remainingUser = 1,
        long roundInterval = 5,
        long waitingInterval = 10)
    {
        Id = id;
        Organizer = organizer;
        Prize = prize;
        MaximumUser = maximumUser;
        MinimumUser = minimumUser;
        RemainingUser = remainingUser;
        RoundInterval = roundInterval;
        WaitingInterval = waitingInterval;
    }

    public SessionMetadata(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Id = ToAddress(list, 0);
        Organizer = ToAddress(list, 1);
        Prize = ToAddress(list, 2);
        MaximumUser = ToInt32(list, 3);
        MinimumUser = ToInt32(list, 4);
        RemainingUser = ToInt32(list, 5);
        RoundInterval = ToInt64(list, 6);
        WaitingInterval = ToInt64(list, 7);
    }

    public IValue Bencoded => new List(
        ToValue(Id),
        ToValue(Organizer),
        ToValue(Prize),
        ToValue(MaximumUser),
        ToValue(MinimumUser),
        ToValue(RemainingUser),
        ToValue(RoundInterval),
        ToValue(WaitingInterval));

    public Address Id { get; }

    public Address Organizer { get; }

    public Address Prize { get; }

    public int MaximumUser { get; }

    public int MinimumUser { get; }

    public int RemainingUser { get; }

    public long RoundInterval { get; }

    public long WaitingInterval { get; }
}
