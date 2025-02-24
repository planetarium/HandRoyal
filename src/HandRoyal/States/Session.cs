using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.Extensions;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(0)]
public sealed record class Session : IEquatable<Session>
{
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
            StartHeight = Metadata.WaitingInterval + height,
            Height = height,
        },
        SessionState.Ready => StartSession(height, random),
        SessionState.Active => PlayRound(height, random),
        SessionState.Ended => null,
        _ => throw new InvalidOperationException($"Invalid session state: {State}"),
    };

    public static Session FromState(IWorldContext world, Address sessionId)
    {
        var sessionsAccount = world[Addresses.Sessions];
        if (!sessionsAccount.TryGetObject<Session>(sessionId, out var session))
        {
            var message = $"Session of id {sessionId} does not exist.";
            throw new ArgumentException(message, nameof(sessionId));
        }

        return session;
    }

    public static Session[] GetSessions(IWorldContext world)
    {
        var sessionsAccount = world[Addresses.Sessions];
        var addressList = sessionsAccount.GetState<List>(Addresses.Sessions, []);

        var sessions = new List<Session>(addressList.Count);
        for (var i = 0; i < addressList.Count; i++)
        {
            var sessionAddress = new Address(addressList[i]);
            if (!sessionsAccount.TryGetObject<Session>(sessionAddress, out var session))
            {
                continue;
            }

            sessions.Add(session);
        }

        return [.. sessions];
    }

    public bool Equals(Session? other) => SerializerUtility.Equals(this, other);

    public override int GetHashCode() => SerializerUtility.GetHashCode(this);

    private Session? StartSession(long height, IRandom random)
    {
        var waitingInterval = Metadata.WaitingInterval;
        var minimumUser = Metadata.MinimumUser;
        if (height < CreationHeight + waitingInterval)
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

        var playerIndexes = random.Shuffle(indexes).ToArray();
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
        var roundInterval = Metadata.RoundInterval;
        var remainingUser = Metadata.RemainingUser;
        var nextHeight = Rounds[^1].Height + roundInterval;
        if (height < nextHeight)
        {
            return null;
        }

        var round = Rounds[^1];
        var winers = round.GetWiners(random);
        var losers = Enumerable.Range(0, Players.Length).Except(winers).ToArray();
        var players = Player.SetState(Players, losers, PlayerState.Lose);

        if (winers.Length <= remainingUser)
        {
            return this with
            {
                Players = Player.SetState(players, winers, PlayerState.Won),
                State = SessionState.Ended,
                Height = height,
            };
        }
        else
        {
            var playerIndexes = random.Shuffle(winers).ToArray();
            var nextRound = new Round
            {
                Height = height,
                Matches = Match.Create(playerIndexes),
            };
            return this with
            {
                Players = players,
                Rounds = Rounds.Add(nextRound),
                Height = height,
            };
        }
    }
}
