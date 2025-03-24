using HandRoyal.States;

namespace HandRoyal.Gloves;

public interface IEffect
{
    string Name { get; }
    EffectType Type { get; }
    int Duration { get; }
    void Apply(BattleContext context);
} 