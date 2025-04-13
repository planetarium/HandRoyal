using HandRoyal.Exceptions;
using HandRoyal.Serialization;
using Libplanet.Action;

namespace HandRoyal.Actions;

[ActionType("MintAsset")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class MintAsset : ActionBase
{
    [Property(0)]
    public long Amount { get; set; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (!context.Signer.Equals(Currencies.SinkAddress))
        {
            throw new MintAssetException("Only sink addresses are allowed.");
        }

        world.MintAsset(Currencies.Royal * Amount);
    }
}
