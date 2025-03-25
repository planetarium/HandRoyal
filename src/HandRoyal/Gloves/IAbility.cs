using HandRoyal.Enums;
using HandRoyal.States;

namespace HandRoyal.Gloves;

public interface IAbility
{
    AbilityType AbilityType { get; }

    (Condition AttackerCondition, Condition DefenderCondition) Apply(
        Condition attackerCondition,
        Condition defenderCondition,
        bool isAttackerWin);
}
