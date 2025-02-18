using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("RegisterGlove")]
public sealed record class RegisterGlove : ActionBase
{
    public RegisterGlove()
    {
    }

    public RegisterGlove(IValue value) => Id = new Address(value);

    public required Address Id { get; init; }

    protected override IValue PlainValue => Id.Bencoded;

    protected override void OnExecute(WorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        var glovesAccount = world[Addresses.Gloves];
        if (glovesAccount.Contains(Id))
        {
            throw new RegisterGloveException($"Glove of given id {Id} is already exists.");
        }

        var glove = new Glove(Id, signer);
        glovesAccount[Id] = glove.Bencoded;
    }
}
