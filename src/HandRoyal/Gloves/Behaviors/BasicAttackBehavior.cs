using HandRoyal.Gloves.Abilities;
using HandRoyal.States;

namespace HandRoyal.Gloves.Behaviors;

public class BasicAttackBehavior : IAttackBehavior
{
    public (Condition NextAttackerCondition, Condition NextDefenderCondition) Execute(
        IGlove attackerGlove,
        IGlove? defenderGlove,
        Condition attackerCondition,
        Condition defenderCondition,
        bool isAttackerWin)
    {
        if (!isAttackerWin)
        {
            return (attackerCondition, defenderCondition);
        }

        // 일반적인 가위바위보 로직
        if (defenderGlove is not { } defenderGloveNotNull)
        {
            defenderCondition = defenderCondition with
            {
                HealthPoint = defenderCondition.HealthPoint - attackerGlove.BaseDamage,
            };
        }
        else
        {
            var damage = attackerGlove.BaseDamage;
            foreach (var effect in defenderGloveNotNull!.Abilities)
            {
                if (effect is DamageReductionAbility dre)
                {
                    damage = Math.Max(0, damage - dre.ReductionAmount);
                }
            }

            defenderCondition = defenderCondition with
            {
                HealthPoint = defenderCondition.HealthPoint - damage,
            };
        }

        return (attackerCondition, defenderCondition);
    }
}
