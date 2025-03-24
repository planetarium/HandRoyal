using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public interface IGlove
{
    public Address Id { get; }

    public GloveType Type { get; }

    public Rarity Rarity { get; }

    public int BaseDamage { get; }

    public IEnumerable<IEffect> Effects { get; }

    public IAttackBehavior AttackBehavior { get; }
}
