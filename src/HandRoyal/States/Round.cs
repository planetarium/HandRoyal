using System.Collections.Immutable;
using HandRoyal.Serialization;
using Libplanet.Action;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Round : IEquatable<Round>
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
            var winners = match.GetWinners(random);
            builder.AddRange(winners);
        }

        return builder.ToImmutable();
    }

    public Round Submit(int playerIndex, MoveType move)
    {
        var matches = Matches;
        for (var i = 0; i < matches.Length; i++)
        {
            var match = matches[i];
            if (match.Move1.PlayerIndex == playerIndex)
            {
                match = match with { Move1 = match.Move1 with { Type = move } };
                return this with { Matches = matches.SetItem(i, match) };
            }
            else if (match.Move2 is not null && match.Move2.PlayerIndex == playerIndex)
            {
                match = match with { Move2 = match.Move2 with { Type = move } };
                return this with { Matches = matches.SetItem(i, match) };
            }
        }

        throw new ArgumentException($"Player {playerIndex} is not in this round.");
    }

    public bool Equals(Round? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
