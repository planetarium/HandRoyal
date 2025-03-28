using HandRoyal.States;

namespace HandRoyal.Gloves.Behaviors;

public interface IAttackBehavior
{
    (Condition NextAttackerCondition, Condition NextDefenderCondition) Execute(
        IGlove attackerGlove,
        IGlove? defenderGlove,
        Condition attackerCondition,
        Condition defenderCondition,
        bool isAttackerWin,
        BattleContext context);
}
