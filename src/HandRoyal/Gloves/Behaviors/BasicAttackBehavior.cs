using HandRoyal.Enums;
using HandRoyal.States;
using HandRoyal.Gloves.Effects;

namespace HandRoyal.Gloves.Behaviors;

public class BasicAttackBehavior : IAttackBehavior
{
    public BattleResult Execute(BattleContext context)
    {
        var player1Glove = context.Player1Glove;
        var player2Glove = context.Player2Glove;

        // 특수 장갑의 능력 적용
        if (player1Glove?.Type == GloveType.Special)
        {
            switch (player1Glove.Name)
            {
                case "Gun":
                    var damage1 = player1Glove.BaseDamage;
                    context.ApplyDamageReduction(damage1, true);
                    return new BattleResult(0, context.Player1Condition.HealthPoint, context.Player2Condition.HealthPoint);
                // 다른 특수 장갑들의 능력은 여기에 추가
                default:
                    break;
            }
        }
        if (player2Glove?.Type == GloveType.Special)
        {
            switch (player2Glove.Name)
            {
                case "Gun":
                    var damage2 = player2Glove.BaseDamage;
                    context.ApplyDamageReduction(damage2, false);
                    return new BattleResult(1, context.Player1Condition.HealthPoint, context.Player2Condition.HealthPoint);
                // 다른 특수 장갑들의 능력은 여기에 추가
                default:
                    break;
            }
        }

        // 일반적인 가위바위보 로직
        var winner = Simulator.GetRcpWinner(player1Glove.Type, player2Glove.Type);
        var damage = winner == 0 ? player1Glove.BaseDamage : player2Glove.BaseDamage;
        context.ApplyDamageReduction(damage, winner == 0);

        return new BattleResult(winner, context.Player1Condition.HealthPoint, context.Player2Condition.HealthPoint);
    }
} 