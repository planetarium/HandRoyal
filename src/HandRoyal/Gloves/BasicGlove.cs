using HandRoyal.Enums;
using HandRoyal.Gloves.Behaviors;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public abstract class BasicGlove : IGlove
{
    protected BasicGlove(Address id, GloveType type, int baseDamage)
    {
        Id = id;
        Type = type;
        BaseDamage = baseDamage;
        Effects = Array.Empty<IEffect>();
        AttackBehavior = new BasicAttackBehavior();
    }

    public Address Id { get; }
    public GloveType Type { get; }
    public int BaseDamage { get; }
    public IEnumerable<IEffect> Effects { get; }
    public IAttackBehavior AttackBehavior { get; }
} 