using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.BlockActions;

public sealed class MortgageGas : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        if (context.MaxGasPrice is not { Sign: > 0 } realGasPrice)
        {
            return world;
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
            return world.TransferAsset(
                context, context.Signer, Addresses.MortgagePool, gasToMortgage);
        }

        return world;
    }
}
