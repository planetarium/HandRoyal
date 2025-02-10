using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

namespace LastHandStanding.States;

public sealed record class Player : IBencodable
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
        State = (PlayerState)(int)(Integer)list[2];
    }

    public IValue Bencoded => new List(
        Id.Bencoded,
        Glove.Bencoded,
        (Integer)(int)State);

    public Address Id { get; }

    public Address Glove { get; }

    public PlayerState State { get; set; }

    public static ImmutableArray<Player> SetStateAsDead(
        ImmutableArray<Player> players, int[] winers)
    {
        for (var i = 0; i < players.Length; i++)
        {
            var player = players[i];
            if (!winers.Contains(i))
            {
                players = players.SetItem(i, player with { State = PlayerState.Dead });
            }
        }

        return players;
    }
}
