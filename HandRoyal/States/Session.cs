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
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        Metadata = ToObject<SessionMetadata>(list, 0);
        State = ToEnum<SessionState>(list, 1);
        Players = ToObjects<Player>(list, 2);
        Rounds = ToObjects<Round>(list, 3);
        CreationHeight = ToInt64(list, 4);
        StartHeight = ToInt64(list, 5);
    }

    public IValue Bencoded => new List(
        ToValue(Metadata),
        ToValue(State),
        ToValue(Players),
        ToValue(Rounds),
        ToValue(CreationHeight),
        ToValue(StartHeight));

    public SessionMetadata Metadata { get; }

    public SessionState State { get; set; }

    public ImmutableArray<Player> Players { get; set; } = [];

    public ImmutableArray<Round> Rounds { get; set; } = [];

    public long CreationHeight { get; set; }

    public long StartHeight { get; set; }

    public int FindPlayer(Address address)
    {
        for (var i = 0; i < Players.Length; i++)
        {
            if (Players[i].Id == address)
            {
                return i;
            }
        }

        return -1;
    }

    public Session ProcessRound(long height, IRandom random) => State switch
    {
        SessionState.None => this with { State = SessionState.Ready, CreationHeight = height },
        SessionState.Ready => StartSession(height, random),
        SessionState.Active => PlayRound(height, random),
        SessionState.Ended => this,
        _ => throw new InvalidOperationException($"Invalid session state: {State}"),
    };

    private Session StartSession(long height, IRandom random)
    {
        var waitingInterval = Metadata.WaitingInterval;
        var minimumUser = Metadata.MinimumUser;
        if (height < CreationHeight + waitingInterval)
        {
            return this;
        }

        var indexes = Enumerable.Range(0, Players.Length).ToArray();
        if (indexes.Length < minimumUser)
        {
            return this with { State = SessionState.Ended };
        }

        var playerIndexes = random.Shuffle(indexes).ToArray();
        var maches = Match.Create(playerIndexes);
        var round = new Round
        {
            Height = height,
            Index = Rounds.Length,
            Matches = maches,
        };

        return this with
        {
            State = SessionState.Active,
            StartHeight = height,
            Players = Player.SetState(Players, playerIndexes, PlayerState.Playing),
            Rounds = Rounds.Add(round),
        };
    }

    private Session PlayRound(long height, IRandom random)
    {
        var roundInterval = Metadata.RoundInterval;
        var remainingUser = Metadata.RemainingUser;
        var nextHeight = Rounds[^1].Height + roundInterval;
        if (height < nextHeight)
        {
            return this;
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
            };
        }
        else
        {
            var playerIndexes = random.Shuffle(winers).ToArray();
            var nextRound = new Round
            {
                Height = height,
                Index = Rounds.Length,
                Matches = Match.Create(playerIndexes),
            };
            return this with
            {
                Players = players,
                Rounds = Rounds.Add(nextRound),
            };
        }
    }
}
