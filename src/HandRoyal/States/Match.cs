using System.Collections.Immutable;
using HandRoyal.Serialization;
using Libplanet.Action;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Match : StateBase<Match>
{
    [Property(0)]
    public Move Move1 { get; set; } = new();

    [Property(1)]
    public Move Move2 { get; set; } = new();

    public static ImmutableArray<Match> Create(in ImmutableArray<int> players)
    {
        var segmentation = 2;
        var count = (int)Math.Ceiling((double)players.Length / segmentation);
        var builder = ImmutableArray.CreateBuilder<Match>(count);
        for (var i = 0; i < count; i++)
        {
            var start = i * segmentation;
            var end = Math.Min((i + 1) * segmentation, players.Length);
            var playerIndex1 = players[start];
            var playerIndex2 = end - start == segmentation ? players[start + 1] : -1;
            var match = new Match
            {
                Move1 = new Move { PlayerIndex = playerIndex1 },
                Move2 = new Move { PlayerIndex = playerIndex2 },
            };
            builder.Add(match);
        }

        return builder.ToImmutable();
    }

    public int[] GetWinners(IRandom random)
    {
        var move1 = Move1;
        var move2 = Move2;
        if (move1.Type == move2.Type)
        {
            return random.Next(2) == 0
                ? [move1.PlayerIndex]
                : [move2.PlayerIndex];
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
