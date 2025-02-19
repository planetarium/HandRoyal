using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
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

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var sessionsAccount = world[Addresses.Sessions];
        if (!sessionsAccount.TryGetObject<Session>(SessionId, out var session))
        {
            throw new InvalidOperationException($"Session {SessionId} does not exist.");
        }

        var playerIndex = session.FindPlayer(context.Signer);
        if (playerIndex == -1)
        {
            throw new InvalidOperationException("Player is not part of the session.");
        }

        var rounds = session.Rounds;
        var round = rounds[^1];
        round = round.Submit(playerIndex, Move);
        sessionsAccount[SessionId] = session with
        {
            Rounds = rounds.SetItem(rounds.Length - 1, round),
            Height = context.BlockIndex,
        };
    }
}
