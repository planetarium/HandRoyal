using System.Text.Json;
using HandRoyal.Gloves.Behaviors;
using HandRoyal.Gloves.Data;
using HandRoyal.Gloves.Effects;
using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public class GloveFactory
{
    private readonly IReadOnlyDictionary<string, GloveData> _gloveData;
    private readonly IReadOnlyDictionary<string, Type> _behaviorTypes;

    public GloveFactory(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var gloveDataList = JsonSerializer.Deserialize<List<GloveData>>(json) ?? new List<GloveData>();
        _gloveData = gloveDataList.ToDictionary(d => d.Id);

        _behaviorTypes = new Dictionary<string, Type>
        {
            { "Basic", typeof(BasicAttackBehavior) },
            // 여기에 새로운 공격 행동 타입들을 추가할 수 있습니다.
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
            throw new ArgumentException($"Attack behavior type {data.AttackBehaviorType} not found");
        }

        var behavior = (IAttackBehavior)Activator.CreateInstance(behaviorType)!;
        var effects = data.Effects.Select(e => CreateEffect(e)).ToList();

        return new Glove(
            new Address(data.Id),
            data.Type,
            data.Rarity,
            data.BaseDamage,
            effects,
            behavior);
    }

    private IEffect CreateEffect(EffectData data)
    {
        return data.Type switch
        {
            EffectType.DamageReduction => new DamageReductionEffect(
                data.Name,
                data.Duration,
                Convert.ToInt32(data.Parameters["reductionAmount"])),
            EffectType.Burn => new BurnEffect(
                data.Name,
                data.Duration,
                Convert.ToInt32(data.Parameters["damagePerRound"])),
            _ => throw new ArgumentException($"Unknown effect type: {data.Type}")
        };
    }
} 