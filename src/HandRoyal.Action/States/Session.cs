using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.Enums;
using HandRoyal.Extensions;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Session : IEquatable<Session>
{
    [Property(0)]
    public required SessionMetadata Metadata { get; init; }

    [Property(1)]
    public SessionState State { get; init; }

    [Property(2)]
    public ImmutableArray<UserEntry> UserEntries { get; init; } = [];

    [Property(3)]
    public ImmutableArray<Phase> Phases { get; init; } = [];

    [Property(4)]
    public long CreationHeight { get; init; }

    [Property(5)]
    public long StartHeight { get; init; }

    [Property(6)]
    public long Height { get; init; }

    public Session Join(long blockHeight, User user, ImmutableArray<Address> initialGloves)
    {
        if (State != SessionState.Ready)
        {
            var message =
                $"State of the session of id {Metadata.Id} is not READY. " +
                $"(state: {State})";
            throw new InvalidOperationException(message);
        }

        if (Metadata.Users.Length != 0 && !Metadata.Users.Contains(user.Id))
        {
            var message = $"User {user.Id} is not allowed to join the session.";
            throw new InvalidOperationException(message);
        }

        if (UserEntries.Length >= Metadata.MaximumUser)
        {
            var message =
                $"Participant registration of session of id {Metadata.Id} is closed " +
                $"since max user count {Metadata.MinimumUser} has reached.";
            throw new InvalidOperationException(message);
        }

        if (FindUserEntry(user.Id) != -1)
        {
            var message = $"Duplicated participation is prohibited. ({user.Id})";
            throw new InvalidOperationException(message);
        }

        if (user.SessionId != default)
        {
            throw new InvalidOperationException("User is already in a session.");
        }

        var userEntry = new UserEntry
            { Id = user.Id, InitialGloves = initialGloves };
        var userEntries = UserEntries.Add(userEntry);
        return this with
        {
            UserEntries = userEntries,
            Height = blockHeight,
        };
    }

    public Session Submit(long blockHeight, Address userId, int gloveIndex)
    {
        if (State != SessionState.Active)
        {
            var message =
                $"State of the session of id {Metadata.Id} is not ACTIVE. " +
                $"(state: {State})";
            throw new InvalidOperationException(message);
        }

        var userEntryIndex = FindUserEntry(userId);
        if (userEntryIndex == -1)
        {
            var message = $"UserEntry is not part of the session. ({userId})";
            throw new InvalidOperationException(message);
        }

        var phases = Phases;
        var phase = phases[^1];
        phase = phase.Submit(userEntryIndex, gloveIndex);
        return this with
        {
            Phases = phases.SetItem(phases.Length - 1, phase),
            Height = blockHeight,
        };
    }

    public int FindUserEntry(Address useId)
    {
        for (var i = 0; i < UserEntries.Length; i++)
        {
            if (UserEntries[i].Id == useId)
            {
                return i;
            }
        }

        return -1;
    }

    public Session? Process(long height, IRandom random) => State switch
    {
        SessionState.None => this with
        {
            State = SessionState.Ready,
            CreationHeight = height,
            StartHeight = Metadata.StartAfter + height,
            Height = height,
        },
        SessionState.Ready => Start(height, random),
        SessionState.Active => Play(height, random),
        SessionState.Ended => null,
        _ => throw new InvalidOperationException($"Invalid session state: {State}"),
    };

    public static Session GetSession(IWorldContext world, Address sessionId)
    {
        var sessionsAccount = world[Addresses.Sessions];
        if (!sessionsAccount.TryGetValue<Session>(sessionId, out var session))
        {
            var archivedSessionsAccount = world[Addresses.ArchivedSessions];
            if (!archivedSessionsAccount.TryGetValue<Session>(sessionId, out var archivedSession))
            {
                var message = $"Session of id {sessionId} does not exist.";
                throw new ArgumentException(message, nameof(sessionId));
            }

            return archivedSession;
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

    private Session? Start(long height, IRandom random)
    {
        var startAfter = Metadata.StartAfter;
        var minimumUser = Metadata.MinimumUser;
        if (height < CreationHeight + startAfter)
        {
            return null;
        }

        var indices = Enumerable.Range(0, UserEntries.Length).ToArray();
        if (indices.Length < minimumUser)
        {
            return this with
            {
                State = SessionState.Ended,
                Height = height,
            };
        }

        var nextUserEntries = new List<UserEntry>();
        foreach (var userEntry in UserEntries)
        {
            nextUserEntries.Add(userEntry with
            {
                State = UserEntryState.Playing,
            });
        }

        var shuffledUserEntryIndices = random.Shuffle(indices).ToImmutableArray();
        var matches = Match.Create(height, shuffledUserEntryIndices);
        var phase = new Phase
        {
            Height = height,
            Matches = matches,
        };

        return this with
        {
            State = SessionState.Active,
            StartHeight = height,
            Height = height,
            UserEntries = [..nextUserEntries],
            Phases = Phases.Add(phase),
        };
    }

    private Session? Play(long blockIndex, IRandom random)
    {
        var remainingUser = Metadata.RemainingUser;
        var phase = Phases[^1];
        phase = phase with
        {
            Height = blockIndex,
            Matches = [
                ..phase.Matches
                    .Select(match => match.Process(Metadata, UserEntries, blockIndex, random)
                    ?? match)
            ],
        };
        if (!phase.Matches.All(match => match.State == MatchState.Ended))
        {
            return this with
            {
                Phases = Phases[..^1].Add(phase),
            };
        }

        var winnerIndices = phase.GetWinnerIndices(random);
        var losers = Enumerable.Range(0, UserEntries.Length)
            .Except(winnerIndices).ToImmutableArray();
        var userEntries = UserEntry.SetState(UserEntries, losers, UserEntryState.Lose);

        if (winnerIndices.Length <= remainingUser)
        {
            return this with
            {
                UserEntries = UserEntry.SetState(userEntries, winnerIndices, UserEntryState.Won),
                State = SessionState.Ended,
                Height = blockIndex,
                Phases = Phases[..^1].Add(phase),
            };
        }

        var nextPhase = new Phase
        {
            Height = blockIndex,
            Matches = Match.Create(blockIndex, winnerIndices),
        };
        return this with
        {
            UserEntries = userEntries,
            Height = blockIndex,
            Phases = Phases[..^1].Add(phase).Add(nextPhase),
        };
    }
}
