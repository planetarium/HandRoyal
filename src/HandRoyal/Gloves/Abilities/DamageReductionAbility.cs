using HandRoyal.Enums;
using HandRoyal.States;

namespace HandRoyal.Gloves.Abilities;

public class DamageReductionAbility(int reductionAmount) : IAbility
{
    public AbilityType AbilityType => AbilityType.DamageReduction;

    public int ReductionAmount { get; } = reductionAmount;

    public (Condition AttackerCondition, Condition DefenderCondition) Apply(
        Condition attackerCondition,
        Condition defenderCondition,
        bool isAttackerWin) => (attackerCondition, defenderCondition);
}
