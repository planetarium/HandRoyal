using System.Text.Json;
using HandRoyal.Enums;
using HandRoyal.Gloves.Abilities;
using HandRoyal.Gloves.Behaviors;
using HandRoyal.Gloves.Data;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class GloveFactory
{
    private readonly JsonSerializerOptions _jsonOptions = new()
        { PropertyNameCaseInsensitive = true };

    private readonly Dictionary<string, GloveData> _gloveData;
    private readonly Dictionary<string, Type> _behaviorTypes;

    public GloveFactory(string jsonContent)
    {
        var gloveDataList =
            JsonSerializer.Deserialize<GloveDataList>(jsonContent, _jsonOptions);
        _gloveData = gloveDataList.Gloves.ToDictionary(d => d.Id);

        _behaviorTypes = new Dictionary<string, Type>
        {
            { "basic", typeof(BasicAttackBehavior) },
            { "lateCommer", typeof(LateCommerBehavior) },
        };
    }

    public string PickUpGlove(IRandom random, bool ensureUncommon)
    {
        const int common = 256;   // 일반
        const int uncommon = 64;  // 고급
        const int rare = 16;      // 희귀
        const int epic = 4;       // 에픽
        const int legendary = 1;  // 전설
        const int max = common + uncommon + rare + epic + legendary;
        switch (random.Next(0, max))
        {
            case < legendary:
                var legendaries = _gloveData.Values.Where(d => d.Rarity == "legendary").ToArray();
                if (legendaries.Length != 0)
                {
                    var index = random.Next(0, legendaries.Length);
                    return legendaries[index].Id;
                }

                throw new InvalidOperationException("No legendary glove");

            case < epic:
                var epics = _gloveData.Values.Where(d => d.Rarity == "epic").ToArray();
                if (epics.Length != 0)
                {
                    var index = random.Next(0, epics.Length);
                    return epics[index].Id;
                }

                throw new InvalidOperationException("No epic glove");

            case < rare:
                var rares = _gloveData.Values.Where(d => d.Rarity == "rare").ToArray();
                if (rares.Length != 0)
                {
                    var index = random.Next(0, rares.Length);
                    return rares[index].Id;
                }

                throw new InvalidOperationException("No rare glove");

            case < uncommon:
                var uncommons = _gloveData.Values.Where(d => d.Rarity == "uncommon").ToArray();
                if (uncommons.Length != 0)
                {
                    var index = random.Next(0, uncommons.Length);
                    return uncommons[index].Id;
                }

                throw new InvalidOperationException("No uncommon glove");

            default:
                var commons = _gloveData.Values
                    .Where(d => d.Rarity == (ensureUncommon ? "uncommon" : "common"))
                    .ToArray();
                if (commons.Length != 0)
                {
                    var index = random.Next(0, commons.Length);
                    return commons[index].Id;
                }

                throw new InvalidOperationException("No common glove");
        }
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
            "epic" => Rarity.Epic,
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
