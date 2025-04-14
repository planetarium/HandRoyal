using HandRoyal.Game.Gloves;
using Libplanet.Crypto;

namespace HandRoyal.Game.Simulation;

public static class Simulator
{
    // 장갑 소진 및 대미지 계산
    public static (PlayerContext NextPlayerContext1, PlayerContext NextPlayerContext2, int Winner)
        Simulate(
            PlayerContext playerContext1,
            PlayerContext playerContext2,
            BattleContext battleContext)
    {
        // 상태 이상들을 체크하고 상태 이상 대미지를 가함
        var nextContext1 = playerContext1.ApplyEffects(battleContext);
        var nextContext2 = playerContext2.ApplyEffects(battleContext);

        // 배틀 시뮬레이션
        var glove1 = playerContext1.Glove;
        var glove2 = playerContext2.Glove;
        int winner = GetWinner(glove1?.Type, glove2?.Type);

        // 총에 대한 특수 케이스
        var gunAddress = new Address("0x3400000000000000000000000000000000000000");
        if (glove1 is not null)
        {
            var battleResult = winner switch
            {
                -2 => BattleResult.Lose,
                -1 => BattleResult.Draw,
                0 => BattleResult.Win,
                1 => BattleResult.Lose,
                _ => BattleResult.Lose,
            };
            battleResult = glove1.Id.Equals(gunAddress) ? BattleResult.Win : battleResult;
            (nextContext1, nextContext2) =
                glove1.AttackBehavior.Execute(
                    nextContext1,
                    nextContext2,
                    battleResult,
                    battleContext);
            foreach (var ability in glove1.Abilities)
            {
                (nextContext1, nextContext2) = ability.Apply(
                    nextContext1,
                    nextContext2,
                    battleResult,
                    battleContext);
            }
        }

        if (glove2 is not null)
        {
            var battleResult = winner switch
            {
                -2 => BattleResult.Lose,
                -1 => BattleResult.Draw,
                0 => BattleResult.Lose,
                1 => BattleResult.Win,
                _ => BattleResult.Lose,
            };
            battleResult = glove2.Id.Equals(gunAddress) ? BattleResult.Win : battleResult;
            (nextContext2, nextContext1) =
                glove2.AttackBehavior.Execute(
                    nextContext2,
                    nextContext1,
                    battleResult,
                    battleContext);
            foreach (var ability in glove2.Abilities)
            {
                (nextContext2, nextContext1) = ability.Apply(
                    nextContext2,
                    nextContext1,
                    battleResult,
                    battleContext);
            }
        }

        return (nextContext1, nextContext2, winner);

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
