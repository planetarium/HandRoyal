using HandRoyal.Enums;

namespace HandRoyal.Gloves.Data;

public class GloveData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public GloveType Type { get; set; }
    public Rarity Rarity { get; set; }
    public int BaseDamage { get; set; }
    public string AttackBehaviorType { get; set; } = string.Empty;
    public List<EffectData> Effects { get; set; } = new();
}

public class EffectData
{
    public string Name { get; set; } = string.Empty;
    public EffectType Type { get; set; }
    public int Duration { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
} 