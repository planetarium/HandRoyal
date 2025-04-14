using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Abilities;

public class DamageReductionAbility(int reductionAmount) : IAbility
{
    public AbilityType AbilityType => AbilityType.DamageReduction;

    public int ReductionAmount { get; } = reductionAmount;

    // Does nothing
    public (PlayerContext AttackerContext, PlayerContext DefenderContext) Apply(
        PlayerContext attackerContext,
        PlayerContext defenderContext,
        BattleResult battleResult,
        BattleContext context) =>
        (attackerContext, defenderContext);
}
