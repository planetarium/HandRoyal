using HandRoyal.Enums;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public interface IGlove
{
    public Address Id { get; }

    public string Name { get; }

    public GloveType Type { get; }

    public Rarity Rarity { get; }

    public int BaseDamage { get; }

    public IEnumerable<IAbility> Abilities { get; }

    public IAttackBehavior AttackBehavior { get; }
}
