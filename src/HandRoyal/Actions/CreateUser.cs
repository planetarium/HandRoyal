using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.Actions;

[ActionType("CreateUser")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class CreateUser : ActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        if (world.Contains(Addresses.Users, signer))
        {
            throw new InvalidOperationException("User already exists.");
        }

        world[Addresses.Users, signer] = new User
        {
            Id = signer,
            EquippedGlove = default,
        };
    }
}
