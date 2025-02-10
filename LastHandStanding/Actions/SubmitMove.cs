using System;
using Bencodex.Types;
using LastHandStanding.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace LastHandStanding.Actions;

[ActionType("SubmitMove")]
public sealed class SubmitMove : ActionBase
{
    public SubmitMove()
    {
    }

    public SubmitMove(Address sessionId, int value)
    {
        SessionId = sessionId;
        Value = value;
    }

    public Address SessionId { get; set; }

    public int Value { get; set; }

    protected override IValue PlainValueInternal => new List(
        SessionId.Bencoded,
        (Integer)Value);

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
        round = round.Submit(playerIndex, Value);
        session = session with { Rounds = rounds.SetItem(rounds.Length - 1, round) };
        sessionAccount = sessionAccount.SetState(SessionId, session.Bencoded);
        world = world.SetAccount(Addresses.CurrentSession, sessionAccount);
        return world;
    }

    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        var list = (List)plainValueInternal;
        SessionId = new Address(list[0]);
        Value = (int)(Integer)list[1];
    }
}
