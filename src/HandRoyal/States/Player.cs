using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Player : IEquatable<Player>
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public ImmutableArray<Address> InitialGloves { get; init; }

    [Property(2)]
    public ImmutableArray<Address> ActiveGloves { get; init; }

    [Property(3)]
    public PlayerState State { get; set; }

    public static ImmutableArray<Player> SetState(
        ImmutableArray<Player> players, in ImmutableArray<int> winners, PlayerState playerState)
    {
        for (var i = 0; i < players.Length; i++)
        {
            var player = players[i];
            if (winners.Contains(i))
            {
                players = players.SetItem(i, player with { State = playerState });
            }
        }

        return players;
    }

    public bool Equals(Player? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
