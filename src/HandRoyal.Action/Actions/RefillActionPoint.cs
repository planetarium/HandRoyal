using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.Actions;

[ActionType("RefillActionPoint")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class RefillActionPoint : ActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        if (!world.TryGetValue<User>(Addresses.Users, signer, out var user))
        {
            throw new InvalidOperationException("User does not exist.");
        }

        world[Addresses.Users, signer] = user.RefillActionPoint(context.BlockIndex);
    }
}
