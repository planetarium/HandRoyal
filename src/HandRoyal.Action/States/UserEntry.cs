using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class UserEntry : IEquatable<UserEntry>
{
    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public required ImmutableArray<Address> InitialGloves { get; init; }

    [Property(2)]
    public UserEntryState State { get; set; }

    public static ImmutableArray<UserEntry> SetState(
        ImmutableArray<UserEntry> players,
        in ImmutableArray<int> winnerIndices,
        UserEntryState playerState)
    {
        for (var i = 0; i < players.Length; i++)
        {
            var player = players[i];
            if (winnerIndices.Contains(i))
            {
                players = players.SetItem(i, player with { State = playerState });
            }
        }

        return players;
    }

    public bool Equals(UserEntry? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
