using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.BlockActions;

public sealed class RewardGas : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        if (context.MaxGasPrice is not { Sign: > 0 } realGasPrice)
        {
            return world;
        }

        if (GasTracer.GasUsed <= 0)
        {
            return world;
        }

        var gasMortgaged = world.GetBalance(Addresses.MortgagePool, realGasPrice.Currency);
        var gasUsedPrice = realGasPrice * GasTracer.GasUsed;
        var gasToTransfer = gasMortgaged < gasUsedPrice ? gasMortgaged : gasUsedPrice;

        if (gasToTransfer.Sign <= 0)
        {
            return world;
        }

        return world.TransferAsset(
                context, Addresses.MortgagePool, Addresses.GasPool, gasToTransfer);
    }
}
