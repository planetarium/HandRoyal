using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
public sealed class JoinSession : ActionBase
{
    public JoinSession()
    {
    }

    public JoinSession(Address sessionId, Address? glove)
    {
        SessionId = sessionId;
        Glove = glove ?? default;
    }

    public Address SessionId { get; private set; }

    public Address Glove { get; private set; }

    protected override IValue PlainValueInternal => List.Empty
        .Add(SessionId.Bencoded)
        .Add(Glove.Bencoded);

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        if (sessionsAccount.GetState(SessionId) is not { } sessionState)
        {
            throw new JoinSessionException($"Session of id {SessionId} does not exists.");
        }

        var session = new Session(sessionState);
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

        if (session.Players.Any(player => player.Id.Equals(context.Signer)))
        {
            var message = $"Duplicated participation is prohibited. ({context.Signer})";
            throw new JoinSessionException(message);
        }

        var usersAccount = world.GetAccount(Addresses.Users);
        if (usersAccount.GetState(context.Signer) is not { } userState)
        {
            var message = $"User does not exists. ({context.Signer})";
            throw new JoinSessionException(message);
        }

        User user;
        try
        {
            user = new User(userState);
        }
        catch (Exception e)
        {
            throw new JoinSessionException("Exception occurred during JoinSession.", e);
        }

        if (Glove != default && !user.Gloves.Contains(Glove))
        {
            var errMsg = $"Cannot join session with invalid glove {Glove}.";
            throw new JoinSessionException(errMsg);
        }

        var players = session.Players;
        var player = new Player(context.Signer, Glove);
        session = session with { Players = players.Add(player) };
        sessionsAccount = sessionsAccount.SetState(SessionId, session.Bencoded);
        return world.SetAccount(Addresses.Sessions, sessionsAccount);
    }

    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        if (plainValueInternal is not List list)
        {
            throw new CreateSessionException("Given plainValue for CreateSession is not list");
        }

        SessionId = new Address(list[0]);
        Glove = new Address(list[1]);
    }
}
