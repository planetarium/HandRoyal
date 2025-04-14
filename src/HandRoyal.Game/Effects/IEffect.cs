using HandRoyal.Game.Simulation;

namespace HandRoyal.Game.Effects;

public interface IEffect
{
    EffectType EffectType { get; }

    int Duration { get; }

    (IEffect NextEffect, PlayerContext NextContext) Apply(
        PlayerContext context,
        BattleContext battleContext);
}
