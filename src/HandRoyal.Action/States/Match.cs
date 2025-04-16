using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Extensions;
using HandRoyal.Game.RoundRules;
using HandRoyal.Game.Simulation;
using HandRoyal.Loader;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Match
{
    [Property(0)]
    public long StartHeight { get; init; }

    [Property(1)]
    public ImmutableArray<int> UserEntryIndices { get; init; }

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

            if (UserEntryIndices[1] == -1)
            {
                // 부전승
                return UserEntryIndices[0];
            }

            var round = Rounds[^1];
            if ((round.Player1.HealthPoint <= 0 && round.Player2.HealthPoint <= 0) ||
                round.Player1.HealthPoint == round.Player2.HealthPoint)
            {
                // 둘 다 0 이하거나 체력이 같으면 무승부
                return -1;
            }

            // 남은 체력이 많은 사람이 승리 (0이하인 경우도 포함)
            return round.Player1.HealthPoint > round.Player2.HealthPoint
                ? UserEntryIndices[0]
                : UserEntryIndices[1];
        }
    }

    public static ImmutableArray<Match> Create(
        long blockIndex,
        in ImmutableArray<int> userEntryIndices)
    {
        var segmentation = 2;
        var count = (int)Math.Ceiling((double)userEntryIndices.Length / segmentation);
        var builder = ImmutableArray.CreateBuilder<Match>(count);
        for (var i = 0; i < count; i++)
        {
            var start = i * segmentation;
            var end = Math.Min((i + 1) * segmentation, userEntryIndices.Length);
            var userEntryIndex1 = userEntryIndices[start];
            var userEntryIndex2 = end - start == segmentation ? userEntryIndices[start + 1] : -1;
            var match = new Match
            {
                StartHeight = blockIndex,
                State = MatchState.None,
                UserEntryIndices = [userEntryIndex1, userEntryIndex2],
                Rounds = [],
            };
            builder.Add(match);
        }

        return builder.ToImmutable();
    }

    public Match? Submit(int userEntryIndex, int gloveIndex)
    {
        if (UserEntryIndices[0] == userEntryIndex)
        {
            if (State != MatchState.Active || !Rounds.Any())
            {
                throw new InvalidOperationException("Cannot submit since match is inactive.");
            }

            var round = Rounds[^1];
            if (round.Player1.GloveUsed[gloveIndex])
            {
                throw new InvalidOperationException("Cannot reuse glove.");
            }

            CheckSubmissionByRoundRule(round.Player1.InitialGloves[gloveIndex]);

            var nextRound = round with
            {
                Player1 = round.Player1 with { Submission = gloveIndex },
            };
            return this with
            {
                Rounds = Rounds.SetItem(Rounds.Length - 1, nextRound),
            };
        }
        else if (UserEntryIndices[1] == userEntryIndex)
        {
            if (State != MatchState.Active || !Rounds.Any())
            {
                throw new InvalidOperationException("Cannot submit since match is inactive.");
            }

            var round = Rounds[^1];
            if (round.Player2.GloveUsed[gloveIndex])
            {
                throw new InvalidOperationException("Cannot reuse glove.");
            }

            CheckSubmissionByRoundRule(round.Player2.InitialGloves[gloveIndex]);

            var nextRound = round with
            {
                Player2 = round.Player2 with { Submission = gloveIndex },
            };
            return this with
            {
                Rounds = Rounds.SetItem(Rounds.Length - 1, nextRound),
            };
        }

        return null;
    }

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    public bool Equals(Match? other) => ModelUtility.Equals(this, other);

    internal ImmutableArray<IRoundRule> GetRoundRules()
    {
        return [..Rounds.Select(r => RoundRuleLoader.CreateRoundRule(r.RoundRuleData))];
    }

    internal Match? Process(
        SessionMetadata metadata,
        in ImmutableArray<UserEntry> userEntries,
        long blockIndex,
        IRandom random) => State switch
    {
        MatchState.None => Start(metadata, userEntries, blockIndex, random),
        MatchState.Active => Play(metadata, blockIndex, random),
        MatchState.Break => Break(metadata, blockIndex, random),
        MatchState.Ended => null,
        _ => throw new InvalidOperationException($"Invalid match state: {State}"),
    };

    private Match Start(
        SessionMetadata metadata,
        ImmutableArray<UserEntry> userEntries,
        long blockIndex,
        IRandom random)
    {
        if (UserEntryIndices[1] == -1)
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
                    Player1 = new Player
                    {
                        Address = userEntries[UserEntryIndices[0]].Id,
                        HealthPoint = metadata.InitialHealthPoint,
                        InitialGloves = userEntries[UserEntryIndices[0]].InitialGloves,
                        GloveInactive = [
                            ..random.Shuffle(
                                Enumerable.Range(0, metadata.NumberOfInitialGloves))
                                .Select(i => i >= metadata.NumberOfActiveGloves)],
                        GloveUsed = [
                            ..Enumerable.Range(0, metadata.NumberOfInitialGloves)
                            .Select(_ => false)],
                        Submission = -1,
                    },
                    Player2 = new Player
                    {
                        Address = userEntries[UserEntryIndices[1]].Id,
                        HealthPoint = metadata.InitialHealthPoint,
                        InitialGloves = userEntries[UserEntryIndices[1]].InitialGloves,
                        GloveInactive = [
                            ..random.Shuffle(
                                Enumerable.Range(0, metadata.NumberOfInitialGloves))
                                .Select(i => i >= metadata.NumberOfActiveGloves)],
                        GloveUsed = [..Enumerable.Range(0, metadata.NumberOfInitialGloves)
                            .Select(_ => false)],
                        Submission = -1,
                    },
                    RoundRuleData = new RoundRuleData
                    {
                        Type = RoundRuleType.None,
                        Parameters = ImmutableArray<string>.Empty,
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
        var condition1 = round.Player1;
        var condition2 = round.Player2;
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
            : GloveLoader.LoadGlove(round.Player1.InitialGloves[condition1.Submission]));
        var playerContext2 = condition2.ToPlayerContext(condition2.Submission == -1
            ? null
            : GloveLoader.LoadGlove(round.Player2.InitialGloves[condition2.Submission]));
        int roundIndex = Rounds.Length - 1;
        var battleContext = new BattleContext
        {
            RoundIndex = roundIndex,
            RoundRules = GetRoundRules(),
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
            0 => UserEntryIndices[0],
            1 => UserEntryIndices[1],
            _ => throw new InvalidOperationException($"Invalid winner: {winnerRaw}"),
        };
        round = round with
        {
            Player1 = condition1 with
            {
                HealthPoint = nextPlayerContext1.HealthPoint,
                ActiveEffectData = [..nextPlayerContext1.Effects.Select(EffectLoader.ToEffectData)],
            },
            Player2 = condition2 with
            {
                HealthPoint = nextPlayerContext2.HealthPoint,
                ActiveEffectData = [..nextPlayerContext2.Effects.Select(EffectLoader.ToEffectData)],
            },
            Winner = winner,
        };
        return this with
        {
            State = MatchState.Break,
            Rounds = Rounds.SetItem(roundIndex, round),
        };
    }

    private Match? Break(SessionMetadata metadata, long height, IRandom random)
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
            Player1 = round.Player1 with { Submission = -1 },
            Player2 = round.Player2 with { Submission = -1 },
        };
        if (round.Player1.HealthPoint <= 0 || round.Player2.HealthPoint <= 0)
        {
            return this with
            {
                State = MatchState.Ended,
            };
        }

        round = round with
        {
            Winner = -2,
            Player1 = round.Player1 with { Submission = -1 },
            Player2 = round.Player2 with { Submission = -1 },

            // Generate Round rule in round 3, 5
            // Todo: Prevent duplicated round rule?
            RoundRuleData = (Rounds.Length % 2 == 1) ?
                RoundRuleLoader.GenerateRandomRoundRuleData(random) :
                new RoundRuleData
                    { Type = RoundRuleType.None, Parameters = ImmutableArray<string>.Empty },
        };

        return this with
        {
            State = MatchState.Active,
            Rounds = Rounds.Add(round),
        };
    }

    private void CheckSubmissionByRoundRule(Address submission)
    {
        var glove = GloveLoader.LoadGlove(submission);
        foreach (var rule in GetRoundRules())
        {
            if (rule is BanGloveTypeRule banGloveTypeRule &&
                glove.Type == banGloveTypeRule.BannedGloveType)
            {
                throw new InvalidOperationException(
                    $"Cannot submit banned glove type: {glove.Type}");
            }
        }
    }
}
