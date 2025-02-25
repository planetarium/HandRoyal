using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("SubmitMove")]
[Model(Version = 1)]
public sealed record class SubmitMove : ActionBase
{
    [Property(0)]
    public required Address SessionId { get; init; }

    [Property(1)]
    public required MoveType Move { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var session = (Session)world[Addresses.Sessions, SessionId];
        var playerIndex = session.FindPlayer(context.Signer);
        if (playerIndex == -1)
        {
            throw new InvalidOperationException("Player is not part of the session.");
        }

        var rounds = session.Rounds;
        var round = rounds[^1];
        round = round.Submit(playerIndex, Move);
        world[Addresses.Sessions, SessionId] = session with
        {
            Rounds = rounds.SetItem(rounds.Length - 1, round),
            Height = context.BlockIndex,
        };
    }
}
