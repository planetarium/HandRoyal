using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("RegisterGlove")]
public class RegisterGlove(Address id) : ActionBase
{
    public RegisterGlove()
        : this(default)
    {
    }

    public Address Id { get; private set; } = id;

    protected override IValue PlainValueInternal => Id.Bencoded;

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var gloveAccount = world.GetAccount(Addresses.Gloves);
        if (gloveAccount.GetState(Id) is not null)
        {
            throw new RegisterGloveException($"Glove of given id {Id} is already exists.");
        }

        var glove = new Glove(Id, context.Signer);
        gloveAccount = gloveAccount.SetState(Id, glove.Bencoded);
        return world.SetAccount(Addresses.Gloves, gloveAccount);
    }

    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
        Id = new Address(plainValueInternal);
    }
}
