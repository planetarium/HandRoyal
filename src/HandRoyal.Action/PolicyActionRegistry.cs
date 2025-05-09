﻿using System.Collections.Immutable;
using HandRoyal.BlockActions;
using Libplanet.Action;

namespace HandRoyal;

public class PolicyActionRegistry : IPolicyActionsRegistry
{
    public ImmutableArray<IAction> BeginBlockActions { get; } =
    [
        new PreProcessSession(),
        new ProcessSession(),
    ];

    public ImmutableArray<IAction> EndBlockActions { get; } =
    [
        new PostProcessSession(),
        new ProcessMatching(),
    ];

    public ImmutableArray<IAction> BeginTxActions { get; } =
    [
        new MortgageGas(),
    ];

    public ImmutableArray<IAction> EndTxActions { get; } =
    [
        new RewardGas(),
        new RefundGas(),
    ];
}
