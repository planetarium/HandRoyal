using HandRoyal.Enums;
using HandRoyal.States;

namespace HandRoyal.Gloves.Effects;

public class DamageReductionEffect : IEffect
{
    public string Name { get; }
    public EffectType Type => EffectType.DamageReduction;
    public int Duration { get; set; }
    public int ReductionAmount { get; }

    public DamageReductionEffect(string name, int duration, int reductionAmount)
    {
        Name = name;
        Duration = duration;
        ReductionAmount = reductionAmount;
    }

    public void Apply(BattleContext context)
    {
        // 대미지 감소 효과는 BattleContext에서 처리됩니다.
        // 이 메서드는 현재 사용되지 않습니다.
    }
} 