using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.Serialization;

internal sealed record class SerializationHeader : IBencodable
{
    private const int ElementCount = 3;

    public int TypeValue { get; init; }

    public string TypeName { get; init; } = string.Empty;

    public int Version { get; set; }

    public IValue Bencoded => new List(
        ToValue(TypeValue),
        ToValue(TypeName),
        ToValue(Version));

    public static bool TryGetHeader(
        IValue value, [MaybeNullWhen(false)] out SerializationHeader header)
    {
        if (value is List list
            && list.Count == ElementCount
            && list[0] is Integer magicValue
            && list[1] is Text typeName
            && list[2] is Integer version)
        {
            header = new SerializationHeader
            {
                TypeValue = magicValue,
                TypeName = typeName,
                Version = version,
            };
            return true;
        }

        header = default;
        return false;
    }

    public static SerializationHeader GetHeader(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException("The value is not a list.", nameof(value));
        }

        if (list.Count != ElementCount)
        {
            throw new ArgumentException("The list does not have two elements.", nameof(value));
        }

        return new SerializationHeader
        {
            TypeValue = ToInt32(list, 0),
            TypeName = GetString(list, 1),
            Version = ToInt32(list, 2),
        };
    }
}
