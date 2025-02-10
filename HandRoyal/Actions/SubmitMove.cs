using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("SubmitMove")]
public sealed class SubmitMove : ActionBase
{
    public SubmitMove()
    {
    }

    public SubmitMove(Address sessionId, MoveType move)
    {
        SessionId = sessionId;
        Move = move;
    }

    public Address SessionId { get; set; }

    public MoveType Move { get; set; }

    protected override IValue PlainValueInternal => new List(
        SessionId.Bencoded,
        (Integer)(int)Move);

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var sessionAccount = world.GetAccount(Addresses.Sessions);
        if (sessionAccount.GetState(SessionId) is not { } sessionState)
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
        session = session with { Rounds = rounds.SetItem(rounds.Length - 1, round) };
        sessionAccount = sessionAccount.SetState(SessionId, session.Bencoded);
        world = world.SetAccount(Addresses.CurrentSession, sessionAccount);
        return world;
    }

    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        var list = (List)plainValueInternal;
        SessionId = new Address(list[0]);
        Move = (MoveType)(int)(Integer)list[1];
    }
}
