using HandRoyal.Exceptions;
using HandRoyal.Gloves;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal.Actions;

[ActionType("PickUp")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class PickUp : ActionBase
{
    public const int Price = 10;

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        if (!world.TryGetValue<User>(Addresses.Users, signer, out var user))
        {
            throw new PickUpException("User does not exist.");
        }

        world.TransferAsset(
            signer,
            Currencies.SinkAddress,
            new FungibleAssetValue(Currencies.Royal, Price, 0));
        var pickup = GloveLoader.PickUpGlove(context.GetRandom());
        bool added = false;
        var nextOwnedGloves = new List<GloveInfo>();
        foreach (var gloveInfo in user.OwnedGloves)
        {
            if (gloveInfo.Id.ToString() == pickup)
            {
                nextOwnedGloves.Add(
                    gloveInfo with { Count = gloveInfo.Count + 1 });
                added = true;
            }
            else
            {
                nextOwnedGloves.Add(gloveInfo);
            }
        }

        if (!added)
        {
            nextOwnedGloves.Add(new GloveInfo { Id = new Address(pickup), Count = 1 });
        }

        world[Addresses.Users, signer] = user with
        {
            OwnedGloves = [..nextOwnedGloves],
        };
    }
}
