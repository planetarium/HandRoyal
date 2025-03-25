using HandRoyal.States;

namespace HandRoyal.Gloves;

public interface IAttackBehavior
{
    (Condition NextAttackerCondition, Condition NextDefenderCondition) Execute(
        IGlove attackerGlove,
        IGlove? defenderGlove,
        Condition attackerCondition,
        Condition defenderCondition,
        bool isAttackerWin);
}
