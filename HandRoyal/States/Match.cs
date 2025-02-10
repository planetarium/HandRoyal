using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;

namespace HandRoyal.States;

public sealed record class Match : IBencodable
{
    public Match()
    {
    }

    public Match(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Move1 = new Move(list[0]);
        Move2 = new Move(list[1]);
    }

    public Move Move1 { get; set; } = new();

    public Move Move2 { get; set; } = new();

    public IValue Bencoded => new List(
        Move1.Bencoded,
        Move2 is not null ? Move2.Bencoded : Null.Value);

    public static ImmutableArray<Match> Create(int[] players)
    {
        var segmentation = 2;
        var count = (int)Math.Ceiling((double)players.Length / segmentation);
        var matchList = new List<Match>(count);
        for (var i = 0; i < count; i++)
        {
            var start = i * segmentation;
            var end = Math.Min((i + 1) * segmentation, players.Length);
            var playerIndex1 = players[start];
            var playerIndex2 = players[end - start] != 2 ? players[end - start] : -1;
            var match = new Match
            {
                Move1 = new Move { PlayerIndex = playerIndex1 },
                Move2 = new Move { PlayerIndex = playerIndex2 },
            };
            matchList.Add(match);
        }

        return [.. matchList];
    }

    public int[] GetWiners()
    {
        var move1 = Move1;
        var move2 = Move2;
        if (move1.Type == move2.Type)
        {
            return [];
        }

        if (move1.Type == MoveType.None)
        {
            return [move2.PlayerIndex];
        }

        if (move2.Type == MoveType.None)
        {
            return [move1.PlayerIndex];
        }

        return move1.Type switch
        {
            MoveType.Rock => move2.Type == MoveType.Scissors
                ? [move1.PlayerIndex]
                : [move2.PlayerIndex],
            MoveType.Scissors => move2.Type == MoveType.Paper
                ? [move1.PlayerIndex]
                : [move2.PlayerIndex],
            MoveType.Paper => move2.Type == MoveType.Rock
                ? [move1.PlayerIndex]
                : [move2.PlayerIndex],
            _ => throw new InvalidCastException("Invalid MoveType"),
        };
    }
}
