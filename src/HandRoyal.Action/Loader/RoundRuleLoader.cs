using System.Collections.Immutable;
using HandRoyal.Game.Gloves;
using HandRoyal.Game.RoundRules;
using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.Loader;

public static class RoundRuleLoader
{
    public static IRoundRule CreateRoundRule(RoundRuleData data)
    {
        return data.Type switch
        {
            RoundRuleType.None => new NonRule(),
            RoundRuleType.BanGloveType => new BanGloveTypeRule(data.Parameters[0] switch
            {
                "rock" => GloveType.Rock,
                "paper" => GloveType.Paper,
                "scissors" => GloveType.Scissors,
                "special" => GloveType.Special,
                _ => throw new ArgumentException($"Invalid glove type: {data.Parameters[0]}"),
            }),
            RoundRuleType.DamageAmplification =>
                new DamageAmplificationRule(int.Parse(data.Parameters[0])),
            _ => throw new ArgumentException($"Unknown round rule type: {data.Type}"),
        };
    }

    public static RoundRuleData GenerateRandomRoundRuleData(IRandom random)
    {
        var type =
            (RoundRuleType)random.Next(0, System.Enum.GetValues(typeof(RoundRuleType)).Length);
        var data = new RoundRuleData
        {
            Type = type,
            Parameters = type switch
            {
                RoundRuleType.None => ImmutableArray<string>.Empty,
                RoundRuleType.BanGloveType => random.Next(0, 4) switch
                {
                    0 => ["rock"],
                    1 => ["paper"],
                    2 => ["scissors"],
                    3 => ["special"],
                    _ => [],
                },
                RoundRuleType.DamageAmplification => [random.Next(-50, 51).ToString()],
                _ => ImmutableArray<string>.Empty,
            },
        };
        return data;
    }
}
