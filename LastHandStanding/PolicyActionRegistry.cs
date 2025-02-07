using System.Collections.Immutable;
using LastHandStanding.BlockActions;
using Libplanet.Action;

namespace LastHandStanding;

public class PolicyActionRegistry : IPolicyActionsRegistry
{
    public ImmutableArray<IAction> BeginBlockActions { get; } =
    [
        new SessionAction(),
    ];

    public ImmutableArray<IAction> EndBlockActions { get; } = [];

    public ImmutableArray<IAction> BeginTxActions { get; } = [];

    public ImmutableArray<IAction> EndTxActions { get; } = [];
}
