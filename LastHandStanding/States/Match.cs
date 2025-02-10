using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;

namespace LastHandStanding.States;

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

        Players = [.. ((List)list[0]).Select(v => (int)(Integer)v)];
        Moves = [.. ((List)list[1]).Select(v => (int)(Integer)v)];
    }

    public ImmutableArray<int> Players { get; set; } = [];

    public ImmutableArray<int> Moves { get; set; } = [];

    public IValue Bencoded => new List(
    [
        new List(Players.Select(p => (Integer)p)),
        new List(Moves.Select(s => (Integer)s)),
    ]);

    public static ImmutableArray<Match> Create(int[] players, int segmentation)
    {
        var count = (int)Math.Ceiling((double)players.Length / segmentation);
        var matchList = new List<Match>(count);
        for (var i = 0; i < count; i++)
        {
            var match = new Match();
            var start = i * segmentation;
            var end = Math.Min((i + 1) * segmentation, players.Length);
            match.Players = [.. players[start..end]];
            match.Moves = [.. Enumerable.Repeat(-1, match.Players.Length)];
            matchList.Add(match);
        }

        return [.. matchList];
    }

    public int[] GetWiners()
    {
        var winerList = new List<int>();
        for (int i = 0; i < Players.Length; i++)
        {
            if (Moves[i] == -1)
            {
                continue; // Automatically lose if move is -1
            }

            bool isWinner = true;
            for (int j = 0; j < Players.Length; j++)
            {
                if (i == j || Moves[j] == -1)
                {
                    continue; // Skip self and players who automatically lose
                }

                // Determine if the current player loses to any other player
                if ((Moves[i] == 0 && Moves[j] == 1) || // 0 vs 1
                    (Moves[i] == 1 && Moves[j] == 2) || // 1 vs 2
                    (Moves[i] == 2 && Moves[j] == 0))   // 2 vs 0
                {
                    isWinner = false;
                    break;
                }
            }

            if (isWinner)
            {
                winerList.Add(Players[i]);
            }
        }

        return [.. winerList];
    }

    public Match Submit(int index, int move)
    {
        if (index < 0 || index >= Players.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be greater than or equal to 0 and less than the number of players.");
        }

        if (move < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(move), "Move must be greater than or equal to 0.");
        }

        if (move > Players.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(move), "Move must be less than the number of players.");
        }

        if (Moves.Contains(move))
        {
            throw new InvalidOperationException($"Move {move} has already been used by another player.");
        }

        return this with { Moves = Moves.SetItem(index, move) };
    }
}
