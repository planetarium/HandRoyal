using HandRoyal.States;

namespace HandRoyal.Gloves;

public interface IAttackBehavior
{
    BattleResult Execute(BattleContext context);
} 