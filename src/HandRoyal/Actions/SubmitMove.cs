using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("SubmitMove")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class SubmitMove : ActionBase
{
    [Property(0)]
    public required Address SessionId { get; init; }

    [Property(1)]
    public required int GloveIndex { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var userId = context.Signer;
        var gloveIndex = GloveIndex;
        var height = context.BlockIndex;
        var session = (Session)world[Addresses.Sessions, SessionId];

        world[Addresses.Sessions, SessionId] = session.Submit(height, userId, gloveIndex);
    }
}
