using HandRoyal.Game.Gloves;

namespace HandRoyal.Game.RoundRules;

public class BanGloveTypeRule : IRoundRule
{
    public BanGloveTypeRule(GloveType gloveType)
    {
        BannedGloveType = gloveType;
    }

    public RoundRuleType Type => RoundRuleType.BanGloveType;

    public GloveType BannedGloveType { get; set; }
}
