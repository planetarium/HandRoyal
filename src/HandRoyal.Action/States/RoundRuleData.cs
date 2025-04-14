using System.Collections.Immutable;
using HandRoyal.Game.RoundRules;
using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
public record RoundRuleData
{
    public required RoundRuleType Type { get; set; }

    public required ImmutableArray<string> Parameters { get; set; }
}
