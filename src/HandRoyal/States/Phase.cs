using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Serialization;
using Libplanet.Action;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Phase : IEquatable<Phase>
{
    [Property(0)]
    public long Height { get; set; }

    [Property(1)]
    public ImmutableArray<Match> Matches { get; set; } = [];

    public ImmutableArray<int> GetWinners(IRandom random)
    {
        var matches = Matches;
        var capacity = Matches.Length * 2;
        var builder = ImmutableArray.CreateBuilder<int>(capacity);
        for (var i = 0; i < matches.Length; i++)
        {
            var match = matches[i];
            var winner = match.Winner;
            if (winner == -1)
            {
                continue;
            }

            builder.Add(winner);
        }

        return builder.ToImmutable();
    }

    public Phase Submit(int playerIndex, int gloveIndex)
    {
        var matches = Matches;
        for (var i = 0; i < matches.Length; i++)
        {
            var match = matches[i].Submit(playerIndex, gloveIndex);
            if (match is not null)
            {
                return this with { Matches = matches.SetItem(i, match) };
            }
        }

        throw new ArgumentException($"Player {playerIndex} is not in this phase.");
    }

    public bool Equals(Phase? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
