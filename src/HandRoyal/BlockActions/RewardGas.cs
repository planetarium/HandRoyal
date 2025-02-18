using Libplanet.Action;

namespace HandRoyal.BlockActions;

public sealed class RewardGas : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (context.MaxGasPrice is not { Sign: > 0 } realGasPrice)
        {
            return;
        }

        if (GasTracer.GasUsed <= 0)
        {
            return;
        }

        var gasMortgaged = world.GetBalance(Addresses.MortgagePool, realGasPrice.Currency);
        var gasUsedPrice = realGasPrice * GasTracer.GasUsed;
        var gasToTransfer = gasMortgaged < gasUsedPrice ? gasMortgaged : gasUsedPrice;

        if (gasToTransfer.Sign <= 0)
        {
            return;
        }

        world.TransferAsset(Addresses.MortgagePool, Addresses.GasPool, gasToTransfer);
    }
}
