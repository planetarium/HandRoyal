using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;
using HandRoyal.Extensions;
using Libplanet.Action;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.States;

public sealed record class Session : IBencodable
{
    public Session(SessionMetadata metadata)
    {
        Metadata = metadata;
    }

    public Session(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.", nameof(value));
        }

        Metadata = ToObject<SessionMetadata>(list, 0);
        State = ToEnum<SessionState>(list, 1);
        Players = ToObjects<Player>(list, 2);
        Rounds = ToObjects<Round>(list, 3);
        CreationHeight = ToInt64(list, 4);
        StartHeight = ToInt64(list, 5);
        Height = ToInt64(list, 6);
    }

    IValue IBencodable.Bencoded => new List(
        ToValue(Metadata),
        ToValue(State),
        ToValue(Players),
        ToValue(Rounds),
        ToValue(CreationHeight),
        ToValue(StartHeight),
        ToValue(Height));

    public SessionMetadata Metadata { get; }

    public SessionState State { get; set; }

    public ImmutableArray<Player> Players { get; set; } = [];

    public ImmutableArray<Round> Rounds { get; set; } = [];

    public long CreationHeight { get; set; }

    public long StartHeight { get; set; }

    public long Height { get; set; }

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
