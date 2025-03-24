using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class BattleContext
{
    public IGlove Player1Glove { get; }
    public IGlove Player2Glove { get; }
    public Condition Player1Condition { get; set; }
    public Condition Player2Condition { get; set; }
    public List<IEffect> ActiveEffects { get; }
    public IRandom Random { get; }
    public int Round { get; private set; }

    public BattleContext(
        IGlove player1Glove,
        IGlove player2Glove,
        Condition player1Condition,
        Condition player2Condition,
        IRandom random)
    {
        Player1Glove = player1Glove;
        Player2Glove = player2Glove;
        Player1Condition = player1Condition;
        Player2Condition = player2Condition;
        ActiveEffects = new List<IEffect>();
        Random = random;
        Round = 1;

        // 장갑의 효과들을 활성화
        foreach (var effect in Player1Glove.Effects)
        {
            if (effect.Duration > 0) // 일시적 효과
            {
                ActiveEffects.Add(effect);
            }
        }
        foreach (var effect in Player2Glove.Effects)
        {
            if (effect.Duration > 0) // 일시적 효과
            {
                ActiveEffects.Add(effect);
            }
        }
    }

    public void NextRound()
    {
        Round++;
    }

    public bool HasBurnEffect(bool isPlayer1)
    {
        var targetGlove = isPlayer1 ? Player1Glove : Player2Glove;
        return ActiveEffects.Any(e => e is BurnEffect && targetGlove.Effects.Contains(e));
    }

    public int GetBurnDamage(bool isPlayer1)
    {
        var targetGlove = isPlayer1 ? Player1Glove : Player2Glove;
        var burnEffect = ActiveEffects.FirstOrDefault(e => e is BurnEffect && targetGlove.Effects.Contains(e));
        return burnEffect != null ? ((BurnEffect)burnEffect).DamagePerRound : 0;
    }

    public void ApplyDamageReduction(int damage, bool isPlayer1)
    {
        var targetCondition = isPlayer1 ? Player1Condition : Player2Condition;
        var activeEffects = ActiveEffects.Where(e => e is DamageReductionEffect).ToList();
        var totalReduction = activeEffects.Sum(e => ((DamageReductionEffect)e).ReductionAmount);
        var reducedDamage = Math.Max(0, damage - totalReduction);

        if (isPlayer1)
        {
            Player1Condition = Player1Condition with { HealthPoint = Player1Condition.HealthPoint - reducedDamage };
        }
        else
        {
            Player2Condition = Player2Condition with { HealthPoint = Player2Condition.HealthPoint - reducedDamage };
        }

        // 화상 효과 적용
        var burnEffects = ActiveEffects.Where(e => e is BurnEffect).ToList();
        foreach (var effect in burnEffects)
        {
            var burnEffect = (BurnEffect)effect;
            if (isPlayer1)
            {
                Player2Condition = Player2Condition with { HealthPoint = Player2Condition.HealthPoint - burnEffect.DamagePerRound };
            }
            else
            {
                Player1Condition = Player1Condition with { HealthPoint = Player1Condition.HealthPoint - burnEffect.DamagePerRound };
            }
        }

        // 효과의 지속 시간 감소 및 제거
        foreach (var effect in activeEffects.Concat(burnEffects))
        {
            if (effect.Duration > 0) // 영구 효과(-1)는 제거하지 않음
            {
                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    ActiveEffects.Remove(effect);
                }
            }
        }
    }
} 