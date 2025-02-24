using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.Serialization;

internal sealed record class SerializationData : IBencodable
{
    private const int ElementCount = 2;

    public required SerializationHeader Header { get; init; }

    public required IValue Value { get; init; }

    public IValue Bencoded => new List(
        ToValue(Header),
        Value);

    public static bool TryGetObject(
        IValue value, [MaybeNullWhen(false)] out SerializationData obj)
    {
        if (value is List list
            && list.Count == ElementCount
            && SerializationHeader.TryGetHeader(list[0], out var header))
        {
            obj = new SerializationData
            {
                Header = header,
                Value = list[1],
            };
            return true;
        }

        obj = default;
        return false;
    }

    public static SerializationData GetObject(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException("The value is not a list.", nameof(value));
        }

        if (list.Count != ElementCount)
        {
            throw new ArgumentException("The list does not have two elements.", nameof(value));
        }

        return new SerializationData
        {
            Header = SerializationHeader.GetHeader(list[0]),
            Value = list[1],
        };
    }
}
