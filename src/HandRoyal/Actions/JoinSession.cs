using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
[Model(Version = 1)]
public sealed record class JoinSession : ActionBase
{
    [Property(0)]
    public required Address SessionId { get; init; }

    [Property(1)]
    public required Address Glove { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        var session = (Session)world[Addresses.Sessions, SessionId];
        var user = (User)world[Addresses.Users, signer];
        var gloveId = Glove;
        var height = context.BlockIndex;

        world[Addresses.Sessions, SessionId] = session.Join(height, user, gloveId);
        world[Addresses.Users, signer] = user with { SessionId = SessionId };
    }
}
