using Libplanet.Action;

namespace HandRoyal.BlockActions;

public sealed class RefundGas : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (context.MaxGasPrice is not { Sign: > 0 } realGasPrice)
        {
            return;
        }

        var remaining = world.GetBalance(Addresses.MortgagePool, realGasPrice.Currency);
        if (remaining.Sign <= 0)
        {
            return;
        }

        world.TransferAsset(Addresses.MortgagePool, context.Signer, remaining);
    }
}
