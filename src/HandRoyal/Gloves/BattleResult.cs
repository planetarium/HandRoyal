namespace HandRoyal.Gloves;

public class BattleResult
{
    public BattleResult(int damage, IEnumerable<IAbility> effects)
    {
        Damage = damage;
        Effects = effects;
    }

    public int Damage { get; }

    public IEnumerable<IAbility> Effects { get; }
}
