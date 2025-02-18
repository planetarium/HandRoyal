using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.Actions;

[ActionType("SubmitMove")]
public sealed record class SubmitMove : ActionBase
{
    public SubmitMove()
    {
    }

    public SubmitMove(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.", nameof(value));
        }

        SessionId = ToAddress(list, 0);
        Move = ToEnum<MoveType>(list, 1);
    }

    public required Address SessionId { get; init; }

    public required MoveType Move { get; init; }

    protected override IValue PlainValue => new List(
        ToValue(SessionId),
        ToValue(Move));

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionsAccount = world.GetAccount(Addresses.Sessions);
        if (sessionsAccount.GetState(SessionId) is not { } sessionState)
        {
            throw new InvalidOperationException($"Session {SessionId} does not exist.");
        }

        var session = new Session(sessionState);
        var playerIndex = session.FindPlayer(context.Signer);
        if (playerIndex == -1)
        {
            throw new InvalidOperationException("Player is not part of the session.");
        }

        var rounds = session.Rounds;
        var round = rounds[^1];
        round = round.Submit(playerIndex, Move);
        session = session with
        {
            Rounds = rounds.SetItem(rounds.Length - 1, round),
            Height = context.BlockIndex,
        };
        sessionsAccount = sessionsAccount.SetState(SessionId, session.Bencoded);
        world = world.SetAccount(Addresses.Sessions, sessionsAccount);
        return world;
    }
}
