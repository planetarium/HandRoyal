using Bencodex.Types;
using LastHandStanding.Exceptions;
using LastHandStanding.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace LastHandStanding.Actions;

[ActionType("StartSession")]
public class StartSession : ActionBase
{
    public StartSession(Address sessionId)
    {
        SessionId = sessionId;
    }
    
    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        SessionId = new Address(plainValueInternal);
    }

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        if (sessionsAccount.GetState(SessionId) is not { } rawSession)
        {
            throw new StartSessionException($"Session of id {SessionId} does not exists.");
        }

        var session = new Session(rawSession);
        if (!session.Organizer.Equals(context.Signer))
        {
            var errMsg =
                $"Non-organizer cannot start session {SessionId}. " +
                $"(Expected: {session.Organizer}, Actual: {context.Signer})";
            throw new StartSessionException(errMsg);
        }
        if (session.State != Session.SessionState.Ready)
        {
            var errMsg =
                $"State of the session of id {SessionId} is not READY. " +
                $"(state: {session.State})";
            throw new StartSessionException(errMsg);
        }

        if (session.Players.Count != Session.MaxUser)
        {
            var errMsg =
                $"Cannot start session {SessionId} since it's participant is not full. " +
                $"(Expected: {Session.MaxUser}, Actual: {session.Players.Count})";
            throw new StartSessionException(errMsg);
        }

        session.State = Session.SessionState.Active;
        sessionsAccount = sessionsAccount.SetState(SessionId, session.Bencoded);
        return world.SetAccount(Addresses.Sessions, sessionsAccount);
    }

    protected override IValue PlainValueInternal => SessionId.Bencoded;

    public Address SessionId { get; private set; }
}