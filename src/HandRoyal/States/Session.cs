using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Bencodex.Types;
using HandRoyal.Extensions;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Session : IEquatable<Session>
{
    [Required]
    [Property(0)]
    public required SessionMetadata Metadata { get; init; }

    [Property(1)]
    public SessionState State { get; init; }

    [Property(2)]
    public ImmutableArray<Player> Players { get; init; } = [];

    [Property(3)]
    public ImmutableArray<Round> Rounds { get; init; } = [];

    [Property(4)]
    public long CreationHeight { get; init; }

    [Property(5)]
    public long StartHeight { get; init; }

    [Property(6)]
    public long Height { get; init; }

    public Session Join(long blockHeight, User user)
    {
        if (State != SessionState.Ready)
        {
            var message =
                $"State of the session of id {Metadata.Id} is not READY. " +
                $"(state: {State})";
            throw new InvalidOperationException(message);
        }

        if (Players.Length >= Metadata.MaximumUser)
        {
            var message =
                $"Participant registration of session of id {Metadata.Id} is closed " +
                $"since max user count {Metadata.MinimumUser} has reached.";
            throw new InvalidOperationException(message);
        }

        if (FindPlayer(user.Id) != -1)
        {
            var message = $"Duplicated participation is prohibited. ({user.Id})";
            throw new InvalidOperationException(message);
        }

        if (user.SessionId != default)
        {
            throw new InvalidOperationException("User is already in a session.");
        }

        var player = new Player { Id = user.Id, Glove = user.EquippedGlove };
        var players = Players.Add(player);
        return this with
        {
            Players = players,
            Height = blockHeight,
        };
    }

    public Session Submit(long blockHeight, Address userId, MoveType move)
    {
        if (State != SessionState.Active)
        {
            var message =
                $"State of the session of id {Metadata.Id} is not ACTIVE. " +
                $"(state: {State})";
            throw new InvalidOperationException(message);
        }

        var playerIndex = FindPlayer(userId);
        if (playerIndex == -1)
        {
            var message = $"Player is not part of the session. ({userId})";
            throw new InvalidOperationException(message);
        }

        var rounds = Rounds;
        var round = rounds[^1];
        round = round.Submit(playerIndex, move);
        return this with
        {
            Rounds = rounds.SetItem(rounds.Length - 1, round),
            Height = blockHeight,
        };
    }

    public int FindPlayer(Address useId)
    {
        for (var i = 0; i < Players.Length; i++)
        {
            if (Players[i].Id == useId)
            {
                return i;
            }
        }

        return -1;
    }

    public Session? ProcessRound(long height, IRandom random) => State switch
    {
        SessionState.None => this with
        {
            State = SessionState.Ready,
            CreationHeight = height,
            StartHeight = Metadata.StartAfter + height,
            Height = height,
        },
        SessionState.Ready => StartSession(height, random),
        SessionState.Active => PlayRound(height, random),
        SessionState.Break => ProcessBreak(height, random),
        SessionState.Ended => null,
        _ => throw new InvalidOperationException($"Invalid session state: {State}"),
    };

    public static Session GetSession(IWorldContext world, Address sessionId)
    {
        var sessionsAccount = world[Addresses.Sessions];
        if (!sessionsAccount.TryGetValue<Session>(sessionId, out var session))
        {
            var message = $"Session of id {sessionId} does not exist.";
            throw new ArgumentException(message, nameof(sessionId));
        }

        return session;
    }

    public static Session[] GetSessions(IWorldContext world)
    {
        var sessionsAccount = world[Addresses.Sessions];
        var addressList = sessionsAccount.GetValue<List>(Addresses.Sessions, []);

        var sessions = new List<Session>(addressList.Count);
        for (var i = 0; i < addressList.Count; i++)
        {
            var sessionAddress = new Address(addressList[i]);
            if (!sessionsAccount.TryGetValue<Session>(sessionAddress, out var session))
            {
                continue;
            }

            sessions.Add(session);
        }

        return [.. sessions];
    }

    public bool Equals(Session? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    private Session? StartSession(long height, IRandom random)
    {
        var startAfter = Metadata.StartAfter;
        var minimumUser = Metadata.MinimumUser;
        if (height < CreationHeight + startAfter)
        {
            return null;
        }

        var indexes = Enumerable.Range(0, Players.Length).ToArray();
        if (indexes.Length < minimumUser)
        {
            return this with
            {
                State = SessionState.Ended,
                Height = height,
            };
        }

        var playerIndexes = random.Shuffle(indexes).ToImmutableArray();
        var matches = Match.Create(playerIndexes);
        var round = new Round
        {
            Height = height,
            Matches = matches,
        };

        return this with
        {
            State = SessionState.Active,
            StartHeight = height,
            Height = height,
            Players = Player.SetState(Players, playerIndexes, PlayerState.Playing),
            Rounds = Rounds.Add(round),
        };
    }

    private Session? PlayRound(long height, IRandom random)
    {
        var roundLength = Metadata.RoundLength;
        var remainingUser = Metadata.RemainingUser;
        var nextHeight = Rounds[^1].Height + roundLength;
        if (height < nextHeight)
        {
            return null;
        }

        var round = Rounds[^1];
        var winners = round.GetWinners(random);
        var losers = Enumerable.Range(0, Players.Length).Except(winners).ToImmutableArray();
        var players = Player.SetState(Players, losers, PlayerState.Lose);

        if (winners.Length <= remainingUser)
        {
            return this with
            {
                Players = Player.SetState(players, winners, PlayerState.Won),
                State = SessionState.Ended,
                Height = height,
            };
        }

        return this with
        {
            State = SessionState.Break,
            Height = height,
        };
    }

    private Session? ProcessBreak(long height, IRandom random)
    {
        var roundLength = Metadata.RoundLength;
        var roundInterval = Metadata.RoundInterval;
        var nextHeight = Rounds[^1].Height + roundLength + roundInterval;
        if (height < nextHeight)
        {
            return null;
        }

        var round = Rounds[^1];
        var winners = round.GetWinners(random);
        var playerIndexes = random.Shuffle(winners).ToImmutableArray();
        var nextRound = new Round
        {
            Height = height,
            Matches = Match.Create(playerIndexes),
        };
        return this with
        {
            Rounds = Rounds.Add(nextRound),
            State = SessionState.Active,
            Height = height,
        };
    }
}
