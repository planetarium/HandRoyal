using HandRoyal.Enums;
using HandRoyal.Exceptions;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("RegisterGlove")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class RegisterGlove : ActionBase
{
    [Property(0)]
    public required Address Id { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (Id == default)
        {
            throw new RegisterGloveException("Cannot register default id");
        }

        if (world.Contains(Addresses.Gloves, Id))
        {
            throw new RegisterGloveException($"Glove of given id {Id} is already exists");
        }

        if (!world[Addresses.Users].TryGetValue<User>(context.Signer, out var user))
        {
            throw new RegisterGloveException($"User of id {context.Signer} does not exist");
        }

        world[Addresses.Gloves, Id] = Id;
        world[Addresses.Users, context.Signer] = user with
        {
            RegisteredGloves = user.RegisteredGloves.Add(Id),
        };
    }
}
