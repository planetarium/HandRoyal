using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.Actions;

[ActionType("CreateUser")]
public sealed record class CreateUser : ActionBase
{
    public CreateUser()
    {
    }

    public CreateUser(IValue value)
        : base(value)
    {
    }

    protected override IValue PlainValue => Null.Value;

    protected override IWorld OnExecute(IActionContext context)
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
}
