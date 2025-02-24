using HandRoyal.Exceptions;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("RegisterGlove")]
[Model(0)]
public sealed record class RegisterGlove : ActionBase
{
    [Property(0)]
    public required Address Id { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (world.Contains(Addresses.Gloves, Id))
        {
            throw new RegisterGloveException($"Glove of given id {Id} is already exists.");
        }

        world[Addresses.Gloves, Id] = new Glove
        {
            Id = Id,
            Author = context.Signer,
        };
    }
}
