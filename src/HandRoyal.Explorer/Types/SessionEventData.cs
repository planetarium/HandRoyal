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
        ? CurrentPhase?.Matches.FirstOrDefault(m => m.Players.Contains(upi))
            ?? throw new InvalidOperationException("User not found in match")
        : null;

    public Address? SessionId => session.Metadata.Id;

    public long Height => session.Height;

    public SessionState SessionState => session.State;

    [GraphSkip]
    public int? UserPlayerIndex => Session.Players
        .Select((p, i) => new { Index = i, Player = p })
        .FirstOrDefault(p => p.Player.Id.Equals(UserId))?.Index;

    [GraphSkip]
    public int? OpponentPlayerIndex => UserPlayerIndex is { } upi
        ? CurrentUserMatch!.Players.FirstOrDefault(p => p != upi)
        : null;

    public Address? OrganizerAddress => session.Metadata.Organizer;

    public Address? OpponentAddress => OpponentPlayerIndex is { } opi
        ? Session.Players[opi].Id
        : null;

    public long CurrentInterval
    {
        get
        {
            if (CurrentUserMatchState == MatchState.Active)
            {
                return Session.Metadata.RoundLength;
            }

            if (CurrentUserMatchState == MatchState.Break)
            {
                return Session.Metadata.RoundInterval;
            }

            return Session.Metadata.StartAfter;
        }
    }

    public Address[]? MyGloves
        => UserPlayerIndex is { } upi
            ? Session.Players[upi].Gloves.ToArray()
            : null;

    public Address[]? OpponentGloves
        => OpponentPlayerIndex is { } opi
            ? Session.Players[opi].Gloves.ToArray()
            : null;

    public Round? CurrentUserRound => CurrentUserMatch?.Rounds.LastOrDefault();

    public MatchState CurrentUserMatchState
        => CurrentUserMatch?.State ?? MatchState.None;

    public PlayerState? PlayerState
        => UserPlayerIndex is { } upi
            ? Session.Players[upi].State
            : null;

    public long IntervalEndHeight
    {
        get
        {
            if (CurrentUserMatch is not { } currentUserMatch)
            {
                return session.StartHeight;
            }

            var roundCount = currentUserMatch.Rounds.Count();

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
