using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Extensions;
using HandRoyal.Game.Simulation;
using HandRoyal.Loader;
using HandRoyal.Serialization;
using Libplanet.Action;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Match
{
    [Property(0)]
    public long StartHeight { get; init; }

    [Property(1)]
    public ImmutableArray<MatchPlayer> MatchPlayers { get; init; }

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

            if (MatchPlayers[1].PlayerIndex == -1)
            {
                // 부전승
                return MatchPlayers[0].PlayerIndex;
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
                ? MatchPlayers[0].PlayerIndex
                : MatchPlayers[1].PlayerIndex;
        }
    }

    public static ImmutableArray<Match> Create(
        long blockIndex,
        in ImmutableArray<Player> players,
        SessionMetadata metadata,
        IRandom random)
    {
        var segmentation = 2;
        var count = (int)Math.Ceiling((double)players.Length / segmentation);
        var builder = ImmutableArray.CreateBuilder<Match>(count);
        for (var i = 0; i < count; i++)
        {
            var start = i * segmentation;
            var end = Math.Min((i + 1) * segmentation, players.Length);
            var playerIndex1 = players[start].PlayerIndex;
            var playerIndex2 = end - start == segmentation ? players[start + 1].PlayerIndex : -1;
            var match = new Match
            {
                StartHeight = blockIndex,
                State = MatchState.None,
                MatchPlayers = [
                    new MatchPlayer
                    {
                        PlayerIndex = playerIndex1,
                        ActiveGloves = [..random.Shuffle(players[playerIndex1].InitialGloves)
                            .Take(metadata.NumberOfActiveGloves)],
                    },
                    new MatchPlayer
                    {
                        PlayerIndex = playerIndex2,
                        ActiveGloves = playerIndex2 == -1
                            ? []
                            : [..random.Shuffle(players[playerIndex2].InitialGloves)
                                .Take(metadata.NumberOfActiveGloves)],
                    }
                ],
                Rounds = [],
            };
            builder.Add(match);
        }

        return builder.ToImmutable();
    }

    public Match? Submit(int playerIndex, int gloveIndex)
    {
        if (MatchPlayers[0].PlayerIndex == playerIndex)
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
        else if (MatchPlayers[1].PlayerIndex == playerIndex)
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
        long blockIndex,
        IRandom random) => State switch
    {
        MatchState.None => Start(metadata, blockIndex),
        MatchState.Active => Play(metadata, blockIndex, random),
        MatchState.Break => Break(metadata, blockIndex),
        MatchState.Ended => null,
        _ => throw new InvalidOperationException($"Invalid match state: {State}"),
    };

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    public bool Equals(Match? other) => ModelUtility.Equals(this, other);

    private Match Start(SessionMetadata metadata, long blockIndex)
    {
        if (MatchPlayers[1].PlayerIndex == -1)
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
        condition1 = condition1 with
        {
            GloveUsed = condition1.Submission == -1
                ? condition1.GloveUsed
                : condition1.GloveUsed.SetItem(condition1.Submission, true),
        };
        condition2 = condition2 with
        {
            GloveUsed = condition2.Submission == -1
                ? condition2.GloveUsed
                : condition2.GloveUsed.SetItem(condition2.Submission, true),
        };

        var playerContext1 = condition1.ToPlayerContext(condition1.Submission == -1
            ? null
            : GloveLoader.LoadGlove(MatchPlayers[0].ActiveGloves[condition1.Submission]));
        var playerContext2 = condition2.ToPlayerContext(condition2.Submission == -1
            ? null
            : GloveLoader.LoadGlove(MatchPlayers[1].ActiveGloves[condition2.Submission]));
        var battleContext = new BattleContext
        {
            RoundIndex = Rounds.Length - 1,
            Random = random,
        };

        var (nextPlayerContext1, nextPlayerContext2, winnerRaw) =
            Simulator.Simulate(
                playerContext1,
                playerContext2,
                battleContext);
        var winner = winnerRaw switch
        {
            -2 or -1 => -1,
            0 => MatchPlayers[0].PlayerIndex,
            1 => MatchPlayers[1].PlayerIndex,
            _ => throw new InvalidOperationException($"Invalid winner: {winnerRaw}"),
        };
        round = round with
        {
            Condition1 = condition1 with
            {
                HealthPoint = nextPlayerContext1.HealthPoint,
                ActiveEffectData = [..nextPlayerContext1.Effects.Select(EffectLoader.ToEffectData)],
            },
            Condition2 = condition2 with
            {
                HealthPoint = nextPlayerContext2.HealthPoint,
                ActiveEffectData = [..nextPlayerContext2.Effects.Select(EffectLoader.ToEffectData)],
            },
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
