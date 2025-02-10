using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;
using LastHandStanding.Extensions;
using Libplanet.Action;
using Libplanet.Crypto;

namespace LastHandStanding.States;

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

        Metadata = new SessionMetadata(list[0]);
        State = (SessionState)(int)(Integer)list[1];
        Players = [.. ((List)list[2]).Select(v => new Player(v))];
        Rounds = [.. ((List)list[3]).Select(v => new Round(v))];
        CreationHeight = (long)(Integer)list[4];
        StartHeight = (long)(Integer)list[5];
    }

    public IValue Bencoded => new List(
        Metadata.Bencoded,
        (Integer)(int)State,
        new List(Players.Select(item => item.Bencoded)),
        new List(Rounds.Select(item => item.Bencoded)),
        (Integer)CreationHeight,
        (Integer)StartHeight);

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
        SessionState.Ended => EndSession(),
        _ => throw new InvalidOperationException($"Invalid session state: {State}"),
    };

    private Session StartSession(long height, IRandom random)
    {
        var waitingInterval = Metadata.WaitingInterval;
        if (height < CreationHeight + waitingInterval)
        {
            return this;
        }

        var indexes = Enumerable.Range(0, Players.Length).ToArray();
        var playerIndexes = random.Shuffle(indexes).ToArray();
        var maches = Match.Create(playerIndexes, 2);
        var round = new Round
        {
            Height = height,
            Index = Rounds.Length,
            Matches = maches,
        };

        static Player SetStateAsPlaying(Player player) => player with
        {
            State = PlayerState.Playing,
        };

        return this with
        {
            State = SessionState.Active,
            StartHeight = height,
            Players = [.. Players.Select(SetStateAsPlaying)],
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
        var winers = round.GetWiners();

        if (winers.Length <= remainingUser)
        {
            return this with
            {
                Players = Player.SetStateAsDead(Players, winers),
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
                Matches = Match.Create(playerIndexes, 2),
            };
            return this with
            {
                Players = Player.SetStateAsDead(Players, winers),
                Rounds = Rounds.Add(nextRound),
            };
        }
    }

    private Session EndSession()
    {
        return this with { State = SessionState.Ended };
    }
}
