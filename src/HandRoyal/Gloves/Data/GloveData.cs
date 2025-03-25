using HandRoyal.Enums;

namespace HandRoyal.Gloves.Data;

public class GloveData
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Rarity { get; set; } = string.Empty;

    public int BaseDamage { get; set; }

    public string AttackBehaviorType { get; set; } = string.Empty;

    public List<AbilityData> Abilities { get; set; } = new();
}
