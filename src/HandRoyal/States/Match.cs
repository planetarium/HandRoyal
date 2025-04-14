using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using HandRoyal.Serialization;
using Libplanet.Action;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Match
{
    [Property(0)]
    public long StartHeight { get; init; }

    [Property(1)]
    public ImmutableArray<int> Players { get; init; }

    [Property(2)]
    public MatchState State { get; init; }

    [Property(3)]
    public ImmutableArray<Round> Rounds { get; init; } = [];

    public int Winner
    {
        get
        {
            if (State != MatchState.Ended)
            {
                return -2;
            }

            if (Players[1] == -1)
            {
                // 부전승
                return Players[0];
            }

            var round = Rounds[^1];
            if ((round.Condition1.HealthPoint <= 0 && round.Condition2.HealthPoint <= 0) ||
                round.Condition1.HealthPoint == round.Condition2.HealthPoint)
            {
                // 둘 다 0 이하거나 체력이 같으면 무승부
                return -1;
            }

            // 남은 체력이 많은 사람이 승리 (0이하인 경우도 포함)
            return round.Condition1.HealthPoint > round.Condition2.HealthPoint
                ? Players[0]
                : Players[1];
        }
    }

    public static ImmutableArray<Match> Create(long blockIndex, in ImmutableArray<int> players)
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
                StartHeight = blockIndex,
                State = MatchState.None,
                Players = [playerIndex1, playerIndex2],
                Rounds = [],
            };
            builder.Add(match);
        }

        return builder.ToImmutable();
    }

    public Match? Submit(int playerIndex, int gloveIndex)
    {
        if (Players[0] == playerIndex)
        {
            if (State != MatchState.Active || !Rounds.Any())
            {
                throw new InvalidOperationException("Cannot submit since match is inactive.");
            }

            var round = Rounds[^1];
            if (round.Condition1.GloveUsed[gloveIndex])
            {
                throw new InvalidOperationException("Cannot reuse glove.");
            }

            var nextRound = round with
            {
                Condition1 = round.Condition1 with { Submission = gloveIndex },
            };
            return this with
            {
                Rounds = Rounds.SetItem(Rounds.Length - 1, nextRound),
            };
        }
        else if (Players[1] == playerIndex)
        {
            if (State != MatchState.Active || !Rounds.Any())
            {
                throw new InvalidOperationException("Cannot submit since match is inactive.");
            }

            var round = Rounds[^1];
            if (round.Condition2.GloveUsed[gloveIndex])
            {
                throw new InvalidOperationException("Cannot reuse glove.");
            }

            var nextRound = round with
            {
                Condition2 = round.Condition2 with { Submission = gloveIndex },
            };
            return this with
            {
                Rounds = Rounds.SetItem(Rounds.Length - 1, nextRound),
            };
        }

        return null;
    }

    public Match? Process(
        SessionMetadata metadata,
        ImmutableArray<Player> players,
        long blockIndex,
        IRandom random) => State switch
    {
        MatchState.None => Start(metadata, blockIndex),
        MatchState.Active => Play(metadata, players, blockIndex, random),
        MatchState.Break => Break(metadata, blockIndex),
        MatchState.Ended => null,
        _ => throw new InvalidOperationException($"Invalid match state: {State}"),
    };

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    public bool Equals(Match? other) => ModelUtility.Equals(this, other);

    private Match Start(SessionMetadata metadata, long blockIndex)
    {
        if (Players[1] == -1)
        {
            return this with
            {
                StartHeight = blockIndex,
                State = MatchState.Ended,
            };
        }

        return this with
        {
            StartHeight = blockIndex,
            State = MatchState.Active,
            Rounds =
            [
                new Round
                {
                    Condition1 = new Condition
                    {
                        HealthPoint = metadata.InitialHealthPoint,
                        GloveUsed = [..Enumerable.Range(0, metadata.MaxRounds).Select(_ => false)],
                        Submission = -1,
                    },
                    Condition2 = new Condition
                    {
                        HealthPoint = metadata.InitialHealthPoint,
                        GloveUsed = [..Enumerable.Range(0, metadata.MaxRounds).Select(_ => false)],
                        Submission = -1,
                    },
                    Winner = -2,
                }
            ],
        };
    }

    private Match? Play(
        SessionMetadata metadata,
        ImmutableArray<Player> players,
        long height,
        IRandom random)
    {
        var roundLength = metadata.RoundLength;
        var roundInterval = metadata.RoundInterval;
        var nextHeight =
            StartHeight + ((Rounds.Length - 1) * (roundLength + roundInterval)) + roundLength;

        if (height < nextHeight)
        {
            return null;
        }

        var round = Rounds[^1];
        var condition1 = round.Condition1;
        var condition2 = round.Condition2;
        var (nextCondition1, nextCondition2, winnerRaw) =
            Simulator.Simulate(
                condition1,
                condition2,
                players[Players[0]].ActiveGloves,
                players[Players[1]].ActiveGloves,
                Rounds.Length - 1,
                random);
        var winner = winnerRaw switch
        {
            -2 or -1 => -1,
            0 => Players[0],
            1 => Players[1],
            _ => throw new InvalidOperationException($"Invalid winner: {winnerRaw}"),
        };
        round = round with
        {
            Condition1 = nextCondition1,
            Condition2 = nextCondition2,
            Winner = winner,
        };
        return this with
        {
            State = MatchState.Break,
            Rounds = Rounds.SetItem(Rounds.Length - 1, round),
        };
    }

    private Match? Break(SessionMetadata metadata, long height)
    {
        var maxRounds = metadata.MaxRounds;
        var roundLength = metadata.RoundLength;
        var roundInterval = metadata.RoundInterval;
        var nextHeight =
            StartHeight + (Rounds.Length * (roundLength + roundInterval));
        if (height < nextHeight)
        {
            return null;
        }

        if (Rounds.Length == maxRounds)
        {
            return this with
            {
                State = MatchState.Ended,
            };
        }

        var round = Rounds[^1];
        round = round with
        {
            Winner = -2,
            Condition1 = round.Condition1 with { Submission = -1 },
            Condition2 = round.Condition2 with { Submission = -1 },
        };
        if (round.Condition1.HealthPoint <= 0 || round.Condition2.HealthPoint <= 0)
        {
            return this with
            {
                State = MatchState.Ended,
            };
        }

        return this with
        {
            State = MatchState.Active,
            Rounds = Rounds.Add(round),
        };
    }
}
