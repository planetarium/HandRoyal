using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.Extensions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.Actions;

[ActionType("CreateSession")]
public sealed record class CreateSession : ActionBase
{
    public CreateSession()
    {
    }

    public CreateSession(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.", nameof(value));
        }

        SessionId = ToAddress(list, 0);
        Prize = ToAddress(list, 1);
        MaximumUser = ToInt32(list, 2);
        MinimumUser = ToInt32(list, 3);
        RemainingUser = ToInt32(list, 4);
        RoundInterval = ToInt64(list, 5);
        WaitingInterval = ToInt64(list, 6);
    }

    public required Address SessionId { get; init; }

    public required Address Prize { get; init; }

    public int MaximumUser { get; init; } = SessionMetadata.Default.MaximumUser;

    public int MinimumUser { get; init; } = SessionMetadata.Default.MinimumUser;

    public int RemainingUser { get; init; } = SessionMetadata.Default.MaximumUser;

    public long RoundInterval { get; init; } = SessionMetadata.Default.RoundInterval;

    public long WaitingInterval { get; init; } = SessionMetadata.Default.WaitingInterval;

    protected override IValue PlainValue => new List(
        ToValue(SessionId),
        ToValue(Prize),
        ToValue(MaximumUser),
        ToValue(MinimumUser),
        ToValue(RemainingUser),
        ToValue(RoundInterval),
        ToValue(WaitingInterval));

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var sessionsAccount = world[Addresses.Sessions];
        if (SessionId == default)
        {
            throw new CreateSessionException("Session id is not set.");
        }

        if (sessionsAccount.ContainsState(SessionId))
        {
            throw new CreateSessionException($"Session of id {SessionId} already exists.");
        }

        var prize = Prize;
        var glovesAccount = world[Addresses.Gloves];
        if (!glovesAccount.TryGetObject<Glove>(prize, out var glove))
        {
            throw new CreateSessionException(
                $"Given glove prize {prize} for session id {SessionId} does not exist.");
        }

        var signer = context.Signer;
        if (!glove.Author.Equals(signer))
        {
            throw new CreateSessionException(
                $"Organizer for session id {SessionId} is not author of the prize {prize}.");
        }

        var sessionMetadata = new SessionMetadata
        {
            Id = SessionId,
            Organizer = signer,
            Prize = Prize,
            MaximumUser = MaximumUser,
            MinimumUser = MinimumUser,
            RemainingUser = RemainingUser,
            RoundInterval = RoundInterval,
            WaitingInterval = WaitingInterval,
        };
        var sessionList = sessionsAccount.GetState(Addresses.Sessions, fallback: List.Empty);
        sessionsAccount[Addresses.Sessions] = sessionList.Add(SessionId);
        sessionsAccount[SessionId] = new Session(sessionMetadata);
    }
}
