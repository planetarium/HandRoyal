using HandRoyal.Enums;
using HandRoyal.Gloves.Behaviors;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class Glove : IGlove
{
    public Glove(
        Address id,
        string name,
        GloveType type,
        Rarity rarity,
        int baseDamage,
        IEnumerable<IAbility> effects,
        IAttackBehavior attackBehavior)
    {
        Id = id;
        Name = name;
        Type = type;
        Rarity = rarity;
        BaseDamage = baseDamage;
        Abilities = effects;
        AttackBehavior = attackBehavior;
    }

    public Address Id { get; }

    public string Name { get; }

    public GloveType Type { get; }

    public Rarity Rarity { get; }

    public int BaseDamage { get; }

    public IEnumerable<IAbility> Abilities { get; }

    public IAttackBehavior AttackBehavior { get; }
}
