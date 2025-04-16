using System.Collections.Immutable;
using HandRoyal.Game.Effects;
using HandRoyal.Game.Gloves;
using HandRoyal.Game.RoundRules;
using HandRoyal.Game.Simulation;
using HandRoyal.Loader;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class Player : IEquatable<Player>
{
    [Property(0)]
    public required Address Address { get; init; }

    [Property(1)]
    public required ImmutableArray<Address> InitialGloves { get; init; }

    [Property(2)]
    public required ImmutableArray<bool> GloveInactive { get; init; }

    [Property(3)]
    public required ImmutableArray<bool> GloveUsed { get; init; }

    [Property(4)]
    public required int HealthPoint { get; init; } = 100;

    [Property(5)]
    public int Submission { get; init; } = -1;

    [Property(6)]
    public ImmutableArray<EffectData> ActiveEffectData { get; init; } =
        ImmutableArray<EffectData>.Empty;

    public ImmutableArray<bool> GetDisabledGloves(Match match)
    {
        var roundRules = match.GetRoundRules();
        var disabledGloves = Enumerable.Range(0, InitialGloves.Length).Select(_ => false);
        foreach (var rule in roundRules)
        {
            if (rule is BanGloveTypeRule bgtr)
            {
                var temp = disabledGloves.ToArray();
                var nextDisabledGloves = InitialGloves.Select((g, index) =>
                    g.GetGloveType() == bgtr.BannedGloveType || temp[index]);
                disabledGloves = nextDisabledGloves;
            }
        }

        return [..disabledGloves];
    }

    public PlayerContext ToPlayerContext(IGlove? glove)
    {
        return new PlayerContext
        {
            HealthPoint = HealthPoint,
            Effects = [..ActiveEffectData.Select(EffectLoader.CreateEffect)],
            Glove = glove,
        };
    }

    public bool Equals(Player? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
