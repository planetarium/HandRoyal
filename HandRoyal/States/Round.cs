using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;

namespace HandRoyal.States;

public sealed record class Round : IBencodable
{
    public Round()
    {
    }

    public Round(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Height = (long)(Integer)list[0];
        Index = (int)(Integer)list[1];
        Matches = [.. ((List)list[2]).Select(v => new Match(v))];
    }

    public long Height { get; set; }

    public int Index { get; set; }

    public ImmutableArray<Match> Matches { get; set; } = [];

    public IValue Bencoded => new List(
        (Integer)Height,
        (Integer)Index,
        new List(Matches.Select(p => p.Bencoded)));

    public int[] GetWiners()
    {
        var matches = Matches;
        var capacity = Matches.Length * 2;
        var winerList = new List<int>(capacity);
        for (var i = 0; i < matches.Length; i++)
        {
            var match = matches[i];
            var winers = match.GetWiners();
            winerList.AddRange(winers);
        }

        return [.. winerList];
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
}
