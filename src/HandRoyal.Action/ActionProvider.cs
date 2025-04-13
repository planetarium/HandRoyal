using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Bencodex.Types;
using HandRoyal.Actions;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.Sys;
using Libplanet.Crypto;
using Libplanet.Node;
using Libplanet.Types.Assets;
using Libplanet.Types.Consensus;

namespace HandRoyal;

public sealed class ActionProvider : IActionProvider
{
    public IActionLoader ActionLoader { get; } = new ActionLoader();

    public IPolicyActionsRegistry PolicyActionsRegistry { get; } = new PolicyActionRegistry();

    public IAction[] GetGenesisActions(Address genesisAddress, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select(item => new Validator(item, new BigInteger(1000)))
            .ToArray();
        var validatorSet = new ValidatorSet(validators: [.. validators]);
        return
        [
            new Initialize(
                validatorSet: validatorSet,
                states: ImmutableDictionary.Create<Address, IValue>()),
            new InitializeRoyal(),
        ];
    }

    [ActionType("InitializeRoyal")]
    [Model(Version = 1)]
    private sealed record class InitializeRoyal : ActionBase
    {
        protected override void OnExecute(IWorldContext world, IActionContext context)
        {
            if (context.BlockIndex != 0)
            {
                throw new InvalidOperationException(
                    $"{nameof(Initialize)} action can be executed only genesis block.");
            }

            world.MintAsset(Currency.Uncapped("Royal", 18, null) * 10000000);
        }
    }
}
