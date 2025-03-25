using System.Collections.Immutable;
using HandRoyal.Gloves;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

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
            OwnedGloves = [
                ..(GloveInfo[])[
                    new GloveInfo
                    {
                        Id = new Address("0x0000000000000000000000000000000000000000"),
                        Count = 2,
                    },
                    new GloveInfo
                    {
                        Id = new Address("0x0000000000000000000000000000000000000001"),
                        Count = 2,
                    },
                    new GloveInfo
                    {
                        Id = new Address("0x0000000000000000000000000000000000000002"),
                        Count = 2,
                    },
                ]
            ],
        };
    }
}
