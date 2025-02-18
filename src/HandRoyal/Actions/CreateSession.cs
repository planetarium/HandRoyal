using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.Actions;

[ActionType("CreateSession")]
public sealed class CreateSession : ActionBase
{
    public CreateSession()
    {
    }

    public CreateSession(
        Address sessionId,
        Address prize,
        int maximumUser,
        int minimumUser,
        int remainingUser,
        long roundInterval,
        long waitingInterval)
    {
        SessionId = sessionId;
        Prize = prize;
        MaximumUser = maximumUser;
        MinimumUser = minimumUser;
        RemainingUser = remainingUser;
        RoundInterval = roundInterval;
        WaitingInterval = waitingInterval;
    }

    public Address SessionId { get; private set; }

    public Address Prize { get; private set; }

    public int MaximumUser { get; private set; }

    public int MinimumUser { get; private set; }

    public int RemainingUser { get; private set; }

    public long RoundInterval { get; private set; }

    public long WaitingInterval { get; private set; }

    protected override IValue PlainValueInternal => new List(
        ToValue(SessionId),
        ToValue(Prize),
        ToValue(MaximumUser),
        ToValue(MinimumUser),
        ToValue(RemainingUser),
        ToValue(RoundInterval),
        ToValue(WaitingInterval));

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        if (sessionsAccount.GetState(SessionId) is not null)
        {
            throw new CreateSessionException($"Session of id {SessionId} already exists.");
        }

        var glovesAccount = world.GetAccount(Addresses.Gloves);
        if (glovesAccount.GetState(Prize) is not { } gloveState)
        {
            throw new CreateSessionException(
                $"Given glove prize {Prize} for session id {SessionId} does not exist.");
        }

        var glove = new Glove(gloveState);
        if (!glove.Author.Equals(context.Signer))
        {
            throw new CreateSessionException(
                $"Organizer for session id {SessionId} is not author of the prize {Prize}.");
        }

        var sessionMetadata = new SessionMetadata(
            SessionId,
            context.Signer,
            Prize,
            MaximumUser,
            MinimumUser,
            RemainingUser,
            RoundInterval,
            WaitingInterval);
        var session = new Session(sessionMetadata);
        var activeSessionAddresses = sessionsAccount.GetState(Addresses.ActiveSessionAddresses)
            is IValue value ? (List)value : [];
        activeSessionAddresses = activeSessionAddresses.Add(SessionId.Bencoded);
        sessionsAccount = sessionsAccount.SetState(
            Addresses.ActiveSessionAddresses, activeSessionAddresses);
        sessionsAccount = sessionsAccount.SetState(SessionId, session.Bencoded);
        return world.SetAccount(Addresses.Sessions, sessionsAccount);
    }

    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        if (plainValueInternal is not List list)
        {
            throw new CreateSessionException("Given plainValue for CreateSession is not list");
        }

        SessionId = ToAddress(list, 0);
        Prize = ToAddress(list, 1);
        MaximumUser = ToInt32(list, 2);
        MinimumUser = ToInt32(list, 3);
        RemainingUser = ToInt32(list, 4);
        RoundInterval = ToInt64(list, 5);
        WaitingInterval = ToInt64(list, 6);
    }
}
