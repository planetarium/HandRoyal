using Bencodex.Types;
using Libplanet.Crypto;

namespace LastHandStanding.States;

public class Session
{
    public const int MaxUser = 100;
    public const int EndUser = 10;
    public const int RoundInterval = 10;
    
    public Session(Address id, Address organizer, Address prize)
    {
        Id = id;
        Organizer = organizer;
        Prize = prize;
        State = SessionState.Ready;
        Players = [];
    }

    public Session(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }
        
        Id = new Address(list[0]);
        Organizer = new Address(list[1]);
        Prize = new Address(list[2]);
        State = (SessionState)(int)(Integer)list[3];
        Players = ((List)list[4]).Select(v => new Player(v)).ToList();
    }

    public IValue Bencoded => List.Empty
        .Add(Id.Bencoded)
        .Add(Organizer.Bencoded)
        .Add(Prize.Bencoded)
        .Add((int)State)
        .Add(new List(Players.Select(user => user.Bencoded)));
    
    public Address Id { get; }
    
    public Address Organizer { get; }
    
    public Address Prize { get; }
    
    public SessionState State { get; set; }
    
    public List<Player> Players { get; }

    public enum SessionState : int
    {
        Ready = 0x00,
        Active = 0x01,
        Ended = 0x02,
    }
}
