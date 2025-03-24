namespace HandRoyal.Gloves;

public class BattleResult
{
    public int Damage { get; }
    public IEnumerable<IEffect> Effects { get; }
    public int Winner { get; }

    public BattleResult(int damage, IEnumerable<IEffect> effects, int winner)
    {
        Damage = damage;
        Effects = effects;
        Winner = winner;
    }
} 