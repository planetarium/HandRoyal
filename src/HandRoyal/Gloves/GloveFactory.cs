using System.Text.Json;
using HandRoyal.Enums;
using HandRoyal.Gloves.Abilities;
using HandRoyal.Gloves.Behaviors;
using HandRoyal.Gloves.Data;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class GloveFactory
{
    private readonly JsonSerializerOptions _jsonOptions = new()
        { PropertyNameCaseInsensitive = true };

    private readonly Dictionary<string, GloveData> _gloveData;
    private readonly Dictionary<string, Type> _behaviorTypes;

    public GloveFactory(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var gloveDataList =
            JsonSerializer.Deserialize<GloveDataList>(json, _jsonOptions);
        _gloveData = gloveDataList.Gloves.ToDictionary(d => d.Id);

        _behaviorTypes = new Dictionary<string, Type>
        {
            { "basic", typeof(BasicAttackBehavior) },
        };
    }

    public IGlove CreateGlove(string gloveId)
    {
        if (!_gloveData.TryGetValue(gloveId, out var data))
        {
            throw new ArgumentException($"Glove with ID {gloveId} not found", nameof(gloveId));
        }

        var behaviorType = _behaviorTypes.GetValueOrDefault(data.AttackBehaviorType);
        if (behaviorType == null)
        {
            throw new ArgumentException(
                $"Attack behavior type {data.AttackBehaviorType} not found");
        }

        var behavior = (IAttackBehavior)Activator.CreateInstance(behaviorType)!;
        var abilities = data.Abilities.Select(CreateAbility).ToList();
        GloveType gloveType = data.Type switch
        {
            "rock" => GloveType.Rock,
            "scissors" => GloveType.Scissors,
            "paper" => GloveType.Paper,
            "special" => GloveType.Special,
            _ => throw new AggregateException($"Unknown glove type {data.Type}"),
        };
        Rarity rarity = data.Rarity switch
        {
            "common" => Rarity.Common,
            "uncommon" => Rarity.Uncommon,
            "rare" => Rarity.Rare,
            "legendary" => Rarity.Legendary,
            _ => throw new AggregateException($"Unknown rarity {data.Rarity}"),
        };

        return new Glove(
            new Address(data.Id),
            data.Name,
            gloveType,
            rarity,
            data.BaseDamage,
            abilities,
            behavior);
    }

    private static IAbility CreateAbility(AbilityData data)
    {
        return data.Type switch
        {
            "damageReduction" => new DamageReductionAbility(
                Convert.ToInt32(data.Parameters[0])),
            "burn" => new BurnAbility(
                Convert.ToInt32(data.Parameters[0])),
            _ => throw new ArgumentException($"Unknown effect type: {data.Type}"),
        };
    }

    private struct GloveDataList
    {
        public GloveDataList()
        {
        }

        public GloveData[] Gloves { get; init; } = [];
    }
}
