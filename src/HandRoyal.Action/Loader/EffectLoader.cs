using HandRoyal.Game.Effects;
using HandRoyal.States;

namespace HandRoyal.Loader;

public static class EffectLoader
{
    public static IEffect CreateEffect(EffectData data)
    {
        return data.Type switch
        {
            EffectType.Burn => new BurnEffect(
                Convert.ToInt32(data.Parameters[0])),
            _ => throw new ArgumentException($"Unknown effect type: {data.Type}"),
        };
    }

    public static EffectData ToEffectData(IEffect effect)
    {
        return effect switch
        {
            BurnEffect b => new EffectData
            {
                Type = b.EffectType,
                Duration = b.Duration,
                Parameters = [b.DamagePerRound],
            },
            _ => throw new ArgumentException($"Unknown effect: {effect}"),
        };
    }
}
