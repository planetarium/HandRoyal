using HandRoyal.Game.RoundRules;

namespace HandRoyal.Explorer.Types;

public record RoundRuleValue
{
    public required RoundRuleType Type { get; init; }

    public required string[] Parameters { get; init; }

    public required int AppliedAt { get; init; }
}
