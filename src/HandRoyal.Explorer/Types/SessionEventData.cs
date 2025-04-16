using GraphQL.AspNet.Attributes;
using HandRoyal.Enums;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

internal sealed class SessionEventData(Session session)
{
    [GraphSkip]
    public Session Session => session;

    [GraphSkip]
    public Address? UserId { get; set; }

    [GraphSkip]
    public Phase? CurrentPhase => Session.Phases.LastOrDefault();

    [GraphSkip]
    public Match? CurrentUserMatch => UserPlayerIndex is { } upi
        ? CurrentPhase?.Matches.FirstOrDefault(m => m.UserEntryIndices.Contains(upi))
        : null;

    public Address? SessionId => session.Metadata.Id;

    public long Height => session.Height;

    public SessionState SessionState => session.State;

    [GraphSkip]
    public int? UserPlayerIndex => Session.UserEntries
        .Select((p, i) => new { Index = i, Player = p })
        .FirstOrDefault(p => p.Player.Id.Equals(UserId))?.Index;

    [GraphSkip]
    public int? OpponentPlayerIndex => UserPlayerIndex is { } upi && CurrentUserMatch is not null
        ? CurrentUserMatch.UserEntryIndices.FirstOrDefault(p => p != upi)
        : null;

    public Address? OrganizerAddress => session.Metadata.Organizer;

    public Address? OpponentAddress
    {
        get
        {
            if (OpponentPlayerIndex is not { } opi)
            {
                return null;
            }

            return opi == -1 ? null : Session.UserEntries[opi].Id;
        }
    }

    public long CurrentInterval
    {
        get
        {
            return CurrentUserMatchState switch
            {
                MatchState.Active => Session.Metadata.RoundLength,
                MatchState.Break => Session.Metadata.RoundInterval,
                _ => Session.Metadata.StartAfter,
            };
        }
    }

    public bool IsPlayer => UserPlayerIndex is not null;

    public int? PlayersLeft =>
        Session.UserEntries.Count(p => p.State == Enums.UserEntryState.Playing);

    public int? CurrentPhaseIndex => Session.Phases.Length - 1;

    public int? CurrentUserRoundIndex => CurrentUserMatch?.Rounds.Length - 1;

    public Player? MyPlayer
        => CurrentUserMatch?.UserEntryIndices[0] == UserPlayerIndex
            ? CurrentUserRound?.Player1
            : CurrentUserRound?.Player2;

    public Player? OpponentPlayer
        => CurrentUserMatch?.UserEntryIndices[0] == UserPlayerIndex
            ? CurrentUserRound?.Player2
            : CurrentUserRound?.Player1;

    public string LastRoundWinner => CurrentUserRound?.Winner switch
    {
        null => "undefined",
        -2 => "playing",
        -1 => "draw",
        { } x when x == UserPlayerIndex => "you",
        { } x when x == OpponentPlayerIndex => "opponent",
        _ => "undefined",
    };

    [GraphSkip]
    public Round? CurrentUserRound => CurrentUserMatch?.Rounds.LastOrDefault();

    public MatchState? CurrentUserMatchState
        => CurrentUserMatch?.State ?? MatchState.None;

    public UserEntryState? UserEntryState
        => UserPlayerIndex is { } upi
            ? Session.UserEntries[upi].State
            : null;

    public long IntervalEndHeight
    {
        get
        {
            if (CurrentUserMatch is not { } currentUserMatch)
            {
                return session.StartHeight;
            }

            if (currentUserMatch.State == MatchState.Ended)
            {
                return currentUserMatch.StartHeight +
                    ((session.Metadata.RoundLength + session.Metadata.RoundInterval) *
                    session.Metadata.MaxRounds);
            }

            var roundCount = currentUserMatch.Rounds.Length;

            var roundInterval = session.Metadata.RoundInterval;
            var roundLength = session.Metadata.RoundLength;

            if (roundCount > 0)
            {
                var roundEndHeight = currentUserMatch.StartHeight
                    + ((roundInterval + roundLength) * roundCount);
                return currentUserMatch.State == MatchState.Active
                    ? roundEndHeight - roundInterval
                    : roundEndHeight;
            }

            return currentUserMatch.StartHeight;
        }
    }
}
