using HandRoyal.Exceptions;
using HandRoyal.Gloves;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal.Actions;

[ActionType("PickUpMany")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class PickUpMany : ActionBase
{
    public const int Count = 10;

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
            new FungibleAssetValue(Currencies.Royal, PickUp.Price * Count, 0));
        var random = context.GetRandom();
        var pickups = Enumerable.Range(0, Count - 1)
            .Select(_ => GloveLoader.PickUpGlove(random, false));
        pickups = pickups.Append(GloveLoader.PickUpGlove(random, true));
        var nextOwnedGloves = new Dictionary<string, int>();
        foreach (var gloveInfo in user.OwnedGloves)
        {
            nextOwnedGloves.Add(gloveInfo.Id.ToString(), gloveInfo.Count);
        }

        foreach (var pickup in pickups)
        {
            if (!nextOwnedGloves.TryAdd(pickup, 1))
            {
                nextOwnedGloves[pickup] += 1;
            }
        }

        world[Addresses.Users, signer] = user with
        {
            OwnedGloves =
            [
                ..nextOwnedGloves.Select(
                    kv => new GloveInfo { Id = new Address(kv.Key), Count = kv.Value })
            ],
        };
    }
}
