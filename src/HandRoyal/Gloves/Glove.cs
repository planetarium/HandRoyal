using HandRoyal.Enums;
using HandRoyal.Gloves.Behaviors;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class Glove : IGlove
{
    public Glove(
        Address id,
        GloveType type,
        Rarity rarity,
        int baseDamage,
        IEnumerable<IEffect> effects,
        IAttackBehavior attackBehavior)
    {
        Id = id;
        Type = type;
        Rarity = rarity;
        BaseDamage = baseDamage;
        Effects = effects;
        AttackBehavior = attackBehavior;
    }

    public Address Id { get; }
    public GloveType Type { get; }
    public Rarity Rarity { get; }
    public int BaseDamage { get; }
    public IEnumerable<IEffect> Effects { get; }
    public IAttackBehavior AttackBehavior { get; }
} 