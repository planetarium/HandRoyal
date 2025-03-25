using HandRoyal.Enums;
using HandRoyal.States;
using HandRoyal.States.Effects;

namespace HandRoyal.Gloves;

public interface IEffect
{
    EffectType EffectType { get; }

    int Duration { get; }

    (IEffect NextEffect, Condition NextCondition) Apply(Condition condition);

    EffectData ToEffectData();
}
