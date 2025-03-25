using HandRoyal.Enums;
using HandRoyal.States;
using HandRoyal.States.Effects;

namespace HandRoyal.Gloves.Abilities;

public class BurnAbility(int damage) : IAbility
{
    public AbilityType AbilityType => AbilityType.Burn;

    public int Duration { get; set;  } = -1;

    public (Condition AttackerCondition, Condition DefenderCondition) Apply(
        Condition attackerCondition,
        Condition defenderCondition,
        bool isAttackerWin)
    {
        var nextDefenderCondition = defenderCondition;
        if (!defenderCondition.ActiveEffects.Any(effect => effect is BurnEffect))
        {
            nextDefenderCondition = nextDefenderCondition.AdjustEffect(new BurnEffect(damage));
        }

        return (attackerCondition, nextDefenderCondition);
    }
}
