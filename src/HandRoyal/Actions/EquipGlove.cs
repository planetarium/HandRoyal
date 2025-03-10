using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("EquipGlove")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class EquipGlove : ActionBase
{
    [Property(0)]
    public required Address Glove { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;

        var user = User.GetUser(world, signer);
        if (user == null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        if (user.EquippedGlove == Glove)
        {
            throw new InvalidOperationException("Glove is already equipped.");
        }

        if (!user.Gloves.Contains(Glove) && Glove != default)
        {
            throw new InvalidOperationException("Glove is not owned.");
        }

        world[Addresses.Users, signer] = new User
        {
            Id = signer,
            Gloves = user.Gloves,
            EquippedGlove = Glove,
            SessionId = user.SessionId,
        };
    }
}
