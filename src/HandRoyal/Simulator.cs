using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;
using HandRoyal.Gloves.Behaviors;
using HandRoyal.Gloves.Data;
using HandRoyal.Gloves.Effects;

namespace HandRoyal;

public static class Simulator
{
    public static (Condition Condition1, Condition Condition2, int Winner) Simulate(
        Condition condition1,
        Condition condition2,
        ImmutableArray<Address> gloves1,
        ImmutableArray<Address> gloves2,
        int gloveIndex1,
        int gloveIndex2,
        IRandom random)
    {
        // 화상 데미지 적용
        var newCondition1 = condition1;
        var newCondition2 = condition2;

        if (newCondition1.HasBurnEffect())
        {
            newCondition1 = newCondition1 with { HealthPoint = newCondition1.HealthPoint - newCondition1.GetBurnDamage() };
        }
        if (newCondition2.HasBurnEffect())
        {
            newCondition2 = newCondition2 with { HealthPoint = newCondition2.HealthPoint - newCondition2.GetBurnDamage() };
        }

        if (gloveIndex1 == -1 && gloveIndex2 == -1)
        {
            return (newCondition1, newCondition2, -1);
        }
        else if (gloveIndex1 == -1)
        {
            IGlove glove2 = GloveLoader.LoadGlove(gloves2[gloveIndex2]);
            var context = new BattleContext(
                null,
                glove2,
                newCondition1,
                newCondition2,
                random);
            
            var result = glove2.AttackBehavior.Execute(context);

            // 화상 효과 적용
            foreach (var effect in glove2.Effects)
            {
                if (effect is BurnEffect burnEffect)
                {
                    var currentBurnDamage = newCondition1.GetBurnDamage();
                    if (burnEffect.DamagePerRound > currentBurnDamage)
                    {
                        newCondition1 = newCondition1 with
                        {
                            ActiveEffects = newCondition1.ActiveEffects
                                .Where(e => e.Type != EffectType.Burn)
                                .Append(new EffectData
                                {
                                    Name = "Burn",
                                    Type = EffectType.Burn,
                                    Duration = -1,
                                    Parameters = new Dictionary<string, object>
                                    {
                                        { "damagePerRound", burnEffect.DamagePerRound }
                                    }
                                })
                                .ToImmutableArray()
                        };
                    }
                }
            }

            return (
                newCondition1 with { HealthPoint = result.Player1Health },
                newCondition2 with
                {
                    GloveUsed = newCondition2.GloveUsed.SetItem(
                        newCondition2.Submission,
                        true)
                },
                1);
        }
        else if (gloveIndex2 == -1)
        {
            IGlove glove1 = GloveLoader.LoadGlove(gloves1[gloveIndex1]);
            var context = new BattleContext(
                glove1,
                null,
                newCondition1,
                newCondition2,
                random);
            
            var result = glove1.AttackBehavior.Execute(context);

            // 화상 효과 적용
            foreach (var effect in glove1.Effects)
            {
                if (effect is BurnEffect burnEffect)
                {
                    var currentBurnDamage = newCondition2.GetBurnDamage();
                    if (burnEffect.DamagePerRound > currentBurnDamage)
                    {
                        newCondition2 = newCondition2 with
                        {
                            ActiveEffects = newCondition2.ActiveEffects
                                .Where(e => e.Type != EffectType.Burn)
                                .Append(new EffectData
                                {
                                    Name = "Burn",
                                    Type = EffectType.Burn,
                                    Duration = -1,
                                    Parameters = new Dictionary<string, object>
                                    {
                                        { "damagePerRound", burnEffect.DamagePerRound }
                                    }
                                })
                                .ToImmutableArray()
                        };
                    }
                }
            }

            return (
                newCondition1 with
                {
                    GloveUsed = newCondition1.GloveUsed.SetItem(
                        newCondition1.Submission,
                        true)
                },
                newCondition2 with { HealthPoint = result.Player2Health },
                0);
        }
        else
        {
            IGlove glove1 = GloveLoader.LoadGlove(gloves1[gloveIndex1]);
            IGlove glove2 = GloveLoader.LoadGlove(gloves2[gloveIndex2]);
            var context = new BattleContext(
                glove1,
                glove2,
                newCondition1,
                newCondition2,
                random);

            var result = glove1.AttackBehavior.Execute(context);

            // 화상 효과 적용
            foreach (var effect in glove1.Effects)
            {
                if (effect is BurnEffect burnEffect)
                {
                    var currentBurnDamage = newCondition2.GetBurnDamage();
                    if (burnEffect.DamagePerRound > currentBurnDamage)
                    {
                        newCondition2 = newCondition2 with
                        {
                            ActiveEffects = newCondition2.ActiveEffects
                                .Where(e => e.Type != EffectType.Burn)
                                .Append(new EffectData
                                {
                                    Name = "Burn",
                                    Type = EffectType.Burn,
                                    Duration = -1,
                                    Parameters = new Dictionary<string, object>
                                    {
                                        { "damagePerRound", burnEffect.DamagePerRound }
                                    }
                                })
                                .ToImmutableArray()
                        };
                    }
                }
            }

            foreach (var effect in glove2.Effects)
            {
                if (effect is BurnEffect burnEffect)
                {
                    var currentBurnDamage = newCondition1.GetBurnDamage();
                    if (burnEffect.DamagePerRound > currentBurnDamage)
                    {
                        newCondition1 = newCondition1 with
                        {
                            ActiveEffects = newCondition1.ActiveEffects
                                .Where(e => e.Type != EffectType.Burn)
                                .Append(new EffectData
                                {
                                    Name = "Burn",
                                    Type = EffectType.Burn,
                                    Duration = -1,
                                    Parameters = new Dictionary<string, object>
                                    {
                                        { "damagePerRound", burnEffect.DamagePerRound }
                                    }
                                })
                                .ToImmutableArray()
                        };
                    }
                }
            }

            return (
                newCondition1 with
                {
                    GloveUsed = newCondition1.GloveUsed.SetItem(
                        newCondition1.Submission,
                        true)
                },
                newCondition2 with
                {
                    GloveUsed = newCondition2.GloveUsed.SetItem(
                        newCondition2.Submission,
                        true)
                },
                result.Winner);
        }
    }

    /// <summary>
    /// Returns the result of rock paper scissors.
    /// </summary>
    /// <param name="glove1"><see cref="GloveType"/> of the glove 1.</param>
    /// <param name="glove2"><see cref="GloveType"/> of the glove 2.</param>
    /// <returns>Returns -2 if one of gloves is special, -1 if drawn,
    /// 0 if glove 1 wins, 1 if glove 2 wins.</returns>
    private static int GetRcpWinner(GloveType glove1, GloveType glove2)
    {
        if (glove1 == glove2)
        {
            return -1;
        }

        return glove1 switch
        {
            GloveType.Rock => glove2 == GloveType.Scissors ? 0 : 1,
            GloveType.Paper => glove2 == GloveType.Rock ? 0 : 1,
            GloveType.Scissors => glove2 == GloveType.Paper ? 0 : 1,
            _ => throw new InvalidOperationException(),
        };
    }
}
