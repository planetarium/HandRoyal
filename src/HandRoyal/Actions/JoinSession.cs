using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
public sealed record class JoinSession : ActionBase
{
    public JoinSession()
    {
    }

    public JoinSession(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.", nameof(value));
        }

        SessionId = ToAddress(list, 0);
        Glove = ToAddress(list, 1);
    }

    public required Address SessionId { get; init; }

    public required Address Glove { get; init; }

    protected override IValue PlainValue => new List(
        ToValue(SessionId),
        ToValue(Glove));

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var sessionsAccount = world[Addresses.Sessions];
        var signer = context.Signer;
        if (!sessionsAccount.TryGetObject<Session>(SessionId, out var session))
        {
            throw new JoinSessionException($"Session of id {SessionId} does not exists.");
        }

        var sessionMetadata = session.Metadata;
        if (session.State != SessionState.Ready)
        {
            var errMsg =
                $"State of the session of id {SessionId} is not READY. " +
                $"(state: {session.State})";
            throw new JoinSessionException(errMsg);
        }

        if (session.Players.Length >= sessionMetadata.MaximumUser)
        {
            var message =
                $"Participant registration of session of id {SessionId} is closed " +
                $"since max user count {sessionMetadata.MinimumUser} has reached.";
            throw new JoinSessionException(message);
        }

        if (session.FindPlayer(signer) != -1)
        {
            var message = $"Duplicated participation is prohibited. ({signer})";
            throw new JoinSessionException(message);
        }

        var usersAccount = world[Addresses.Users];
        if (!usersAccount.TryGetObject<User>(signer, out var user))
        {
            var message = $"User does not exists. ({signer})";
            throw new JoinSessionException(message);
        }

        if (user.SessionId != default)
        {
            throw new JoinSessionException("User is already in a session.");
        }

        if (Glove != default && !user.Gloves.Contains(Glove))
        {
            var errMsg = $"Cannot join session with invalid glove {Glove}.";
            throw new JoinSessionException(errMsg);
        }

        var player = new Player(signer, Glove);
        var players = session.Players.Add(player);
        sessionsAccount[SessionId] = session with { Players = players };
        usersAccount[signer] = user with { SessionId = SessionId };
    }
}
