using System.Collections.Immutable;
using HandRoyal.Game.RoundRules;
using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record RoundRuleData : IEquatable<RoundRuleData>
{
    [Property(0)]
    public required RoundRuleType Type { get; init; }

    [Property(1)]
    public required ImmutableArray<string> Parameters { get; init; }

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    public bool Equals(RoundRuleData? other) => ModelUtility.Equals(this, other);
}
