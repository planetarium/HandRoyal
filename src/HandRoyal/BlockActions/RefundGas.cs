using Libplanet.Action;
using Libplanet.Action.State;

namespace HandRoyal.BlockActions;

public sealed class RefundGas : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        if (context.MaxGasPrice is not { Sign: > 0 } realGasPrice)
        {
            return world;
        }

        var remaining = world.GetBalance(Addresses.MortgagePool, realGasPrice.Currency);
        if (remaining.Sign <= 0)
        {
            return world;
        }

        return world.TransferAsset(
            context, Addresses.MortgagePool, context.Signer, remaining);
    }
}
