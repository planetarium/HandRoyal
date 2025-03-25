using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal;

public static class Simulator
{
    // 장갑 소진 및 대미지 계산
    public static (Condition Condition1, Condition Condition2, int Winner) Simulate(
        Condition condition1,
        Condition condition2,
        ImmutableArray<Address> gloves1,
        ImmutableArray<Address> gloves2,
        IRandom random)
    {
        // 상태 이상들을 체크하고 상태 이상 대미지를 가함
        var nextCondition1 = condition1.ApplyEffects();
        var nextCondition2 = condition2.ApplyEffects();
        var gloveIndex1 = condition1.Submission;
        var gloveIndex2 = condition2.Submission;

        // 배틀 시뮬레이션
        IGlove? glove1 = gloveIndex1 == -1 ? null : GloveLoader.LoadGlove(gloves1[gloveIndex1]);
        IGlove? glove2 = gloveIndex2 == -1 ? null : GloveLoader.LoadGlove(gloves1[gloveIndex2]);
        int winner = GetWinner(glove1?.Type, glove2?.Type);
        var nextGloveUsed1 = gloveIndex1 == -1 ?
            nextCondition1.GloveUsed :
            nextCondition1.GloveUsed.SetItem(gloveIndex1, true);
        var nextGloveUsed2 = gloveIndex2 == -1 ?
            nextCondition2.GloveUsed :
            nextCondition2.GloveUsed.SetItem(gloveIndex2, true);

        // 총에 대한 특수 케이스
        var gunAddress = new Address("0x3400000000000000000000000000000000000000");
        if (glove1 is not null)
        {
            var isAttackerWin = glove1.Id.Equals(gunAddress) || winner == 0;
            (nextCondition1, nextCondition2) =
                glove1.AttackBehavior.Execute(
                    glove1,
                    glove2,
                    nextCondition1,
                    nextCondition2,
                    isAttackerWin);
            foreach (var ability in glove1.Abilities)
            {
                (nextCondition1, nextCondition2) = ability.Apply(
                    nextCondition1,
                    nextCondition2,
                    isAttackerWin);
            }
        }

        if (glove2 is not null)
        {
            (nextCondition2, nextCondition1) =
                glove2.AttackBehavior.Execute(
                    glove2,
                    glove1,
                    nextCondition2,
                    nextCondition1,
                    glove2.Id.Equals(gunAddress) || winner == 1);
        }

        return (
            nextCondition1 with
            {
                GloveUsed = nextGloveUsed1,
            },
            nextCondition2 with
            {
                GloveUsed = nextGloveUsed2,
            },
            winner);

        // 걸린 효과들의 지속시간을 감소
    }

    /// <summary>
    /// Returns the result of rock paper scissors.
    /// </summary>
    /// <param name="glove1"><see cref="GloveType"/> of the glove 1.</param>
    /// <param name="glove2"><see cref="GloveType"/> of the glove 2.</param>
    /// <returns>Returns -2 if one of gloves is special, -1 if drawn,
    /// 0 if glove 1 wins, 1 if glove 2 wins.</returns>
    private static int GetWinner(GloveType? glove1, GloveType? glove2)
    {
        if (glove1 == glove2)
        {
            return -1;
        }

        if (glove1 is null)
        {
            return 1;
        }

        if (glove2 is null)
        {
            return 0;
        }

        if (glove1 == GloveType.Special || glove2 == GloveType.Special)
        {
            return -2;
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
