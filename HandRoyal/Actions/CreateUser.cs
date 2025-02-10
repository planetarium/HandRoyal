using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.Actions;

[ActionType("CreateUser")]
public class CreateUser : ActionBase
{
    public CreateUser()
    {
    }

    protected override IValue PlainValueInternal => Null.Value;

    public override IWorld Execute(IActionContext context)
    {
        var world = context.PreviousState;
        var signer = context.Signer;
        var usersAccount = world.GetAccount(Addresses.Users);
        if (usersAccount.GetState(signer) is not null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new User(signer);
        usersAccount = usersAccount.SetState(signer, user.Bencoded);
        return world.SetAccount(Addresses.Users, usersAccount);
    }

    protected override void LoadPlainValueInternal(IValue plainValueInternal)
    {
    }
}
