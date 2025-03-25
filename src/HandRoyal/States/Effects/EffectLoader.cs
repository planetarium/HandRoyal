using HandRoyal.Enums;
using HandRoyal.Gloves;

namespace HandRoyal.States.Effects;

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
}
