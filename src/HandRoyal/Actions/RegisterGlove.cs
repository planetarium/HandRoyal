using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
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

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var signer = context.Signer;
        var glovesAccount = world.GetAccount(Addresses.Gloves);
        if (glovesAccount.GetState(Id) is not null)
        {
            throw new RegisterGloveException($"Glove of given id {Id} is already exists.");
        }

        var glove = new Glove(Id, signer);
        glovesAccount = glovesAccount.SetState(Id, glove.Bencoded);
        return world.SetAccount(Addresses.Gloves, glovesAccount);
    }
}
