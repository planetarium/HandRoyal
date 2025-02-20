using Libplanet.Action;

namespace HandRoyal.BlockActions;

public sealed class MortgageGas : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (context.MaxGasPrice is not { Sign: > 0 } realGasPrice)
        {
            return;
        }

        var gasOwned = world.GetBalance(context.Signer, realGasPrice.Currency);
        var gasRequired = realGasPrice * GasTracer.GasAvailable;
        var gasToMortgage = gasOwned < gasRequired ? gasOwned : gasRequired;
        if (gasOwned < gasRequired)
        {
            GasTracer.CancelTrace();
        }

        if (gasToMortgage.Sign > 0)
        {
            world.TransferAsset(context.Signer, Addresses.MortgagePool, gasToMortgage);
        }
    }
}
