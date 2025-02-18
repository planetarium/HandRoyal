using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;

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

    protected override void OnExecute(WorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        var usersAccount = world[Addresses.Users];
        if (usersAccount.Contains(signer))
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new User(signer);
        usersAccount[signer] = user.Bencoded;
    }
}
