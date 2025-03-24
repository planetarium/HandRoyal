using System.Collections.Immutable;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

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
        const int damage = 40;
        if (gloveIndex1 == -1 && gloveIndex2 == -1)
        {
            return (condition1, condition2, -1);
        }
        else if (gloveIndex1 == -1)
        {
            return (
                condition1 with { HealthPoint = condition1.HealthPoint - damage },
                condition2 with
                {
                    GloveUsed = condition2.GloveUsed.SetItem(
                        condition2.Submission,
                        true),
                },
                1);
        }
        else if (gloveIndex2 == -1)
        {
            return (
                condition1 with
                {
                    GloveUsed = condition1.GloveUsed.SetItem(
                        condition1.Submission,
                        true),
                },
                condition2 with { HealthPoint = condition2.HealthPoint - damage },
                0);
        }

        IGlove glove1 = GloveLoader.LoadGlove(gloves1[gloveIndex1]);
        IGlove glove2 = GloveLoader.LoadGlove(gloves2[gloveIndex2]);
        var winner = GetRcpWinner(glove1.Type, glove2.Type);
        var newCondition1 = condition1 with
        {
            HealthPoint = condition1.HealthPoint - (winner == 1 ? damage : 0),
            GloveUsed = condition1.GloveUsed.SetItem(
                condition1.Submission,
                true),
        };
        var newCondition2 = condition2 with
        {
            HealthPoint = condition2.HealthPoint - (winner == 0 ? damage : 0),
            GloveUsed = condition2.GloveUsed.SetItem(
                condition2.Submission,
                true),
        };
        return (newCondition1, newCondition2, winner);
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
        if (glove1 == GloveType.Special || glove2 == GloveType.Special)
        {
            return -2;
        }

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
