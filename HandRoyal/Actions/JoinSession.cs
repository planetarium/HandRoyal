using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
public sealed class JoinSession(Address sessionId, Address glove) : ActionBase
{
    public Address SessionId { get; private set; } = sessionId;

    public Address Glove { get; private set; } = glove;

    protected override IValue PlainValueInternal => List.Empty
        .Add(SessionId.Bencoded)
        .Add(Glove.Bencoded);

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        if (sessionsAccount.GetState(SessionId) is not { } rawSession)
        {
            throw new JoinSessionException($"Session of id {SessionId} does not exists.");
        }

        var session = new Session(rawSession);
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
            var errMsg =
                $"Participant registration of session of id {SessionId} is closed " +
                $"since max user count {sessionMetadata.MinimumUser} has reached.";
            throw new JoinSessionException(errMsg);
        }

        if (session.Players.Any(player => player.Id.Equals(context.Signer)))
        {
            var errMsg = $"Duplicated participation is prohibited. ({context.Signer})";
            throw new JoinSessionException(errMsg);
        }

        var usersAccount = world.GetAccount(Addresses.Users);
        if (usersAccount.GetState(context.Signer) is not { } rawUser)
        {
            var errMsg = $"User does not exists. ({context.Signer})";
            throw new JoinSessionException(errMsg);
        }

        User user;
        try
        {
            user = new User(rawUser);
        }
        catch (Exception e)
        {
            throw new JoinSessionException("Exception occured during JoinSession.", e);
        }

        if (!user.Gloves.Contains(Glove))
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
