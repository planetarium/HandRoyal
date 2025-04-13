using HandRoyal.Enums;
using HandRoyal.Game.Effects;
using HandRoyal.Serialization;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record EffectData : IEquatable<EffectData>
{
    [Property(0)]
    public required EffectType Type { get; set; }

    [Property(1)]
    public required int Duration { get; set; }

    [Property(2)]
    public required int[] Parameters { get; set; }

    public bool Equals(EffectData? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
