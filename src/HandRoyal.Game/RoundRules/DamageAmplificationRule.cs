namespace HandRoyal.Game.RoundRules;

public class DamageAmplificationRule : IRoundRule
{
    public DamageAmplificationRule(int multiplier)
    {
        Multiplier = multiplier;
    }

    public RoundRuleType Type => RoundRuleType.DamageAmplification;

    public int Multiplier { get; init; }

    public int CalculateDamage(int damage)
    {
        return damage * (Multiplier + 100) / 100;
    }
}
