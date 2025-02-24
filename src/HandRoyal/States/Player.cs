using System.Collections.Immutable;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(0)]
public sealed record class Player
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public Address Glove { get; init; }

    [Property(2)]
    public PlayerState State { get; set; }

    public static ImmutableArray<Player> SetState(
        ImmutableArray<Player> players, int[] winers, PlayerState playerState)
    {
        for (var i = 0; i < players.Length; i++)
        {
            var player = players[i];
            if (winers.Contains(i))
            {
                players = players.SetItem(i, player with { State = playerState });
            }
        }

        return players;
    }
}
