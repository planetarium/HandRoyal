using HandRoyal.Gloves.Behaviors;

namespace HandRoyal.Gloves.Effects;

public class BurnEffect : IEffect
{
    public string Name { get; }
    public int Duration { get; set; }
    public int DamagePerRound { get; }

    public BurnEffect(string name, int duration, int damagePerRound)
    {
        Name = name;
        Duration = duration;
        DamagePerRound = damagePerRound;
    }

    public void Apply(BattleContext context, bool isWinner)
    {
        // 화상 효과는 승패와 상관없이 적용됩니다.
        context.AddActiveEffect(this);
    }
} 