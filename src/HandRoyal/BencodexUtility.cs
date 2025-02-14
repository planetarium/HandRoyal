using System.Collections.Immutable;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

namespace HandRoyal;

public static class BencodexUtility
{
    public static IValue ToValue(IBencodable value) => value.Bencoded;

    public static IValue ToValue(int value) => new Integer(value);

    public static IValue ToValue(long value) => new Integer(value);

    public static IValue ToValue(Enum @enum) => new Integer((int)(object)@enum);

    public static IValue ToValue<T>(ImmutableArray<T> values)
        where T : IBencodable
        => new List(values.Select(item => item.Bencoded));

    public static Address ToAddress(List list, int index) => new(list[index]);

    public static int ToInt32(List list, int index) => (int)(Integer)list[index];

    public static long ToInt64(List list, int index) => (long)(Integer)list[index];

    public static T ToObject<T>(List list, int index)
        where T : IBencodable
        => ToObject(list, index, CreateInstance<T>);

    public static T ToObject<T>(List list, int index, Func<IValue, T> creator)
        where T : IBencodable
        => creator(list[index]);

    public static ImmutableArray<T> ToObjects<T>(List list, int index)
        where T : IBencodable
        => ToObjects(list, index, CreateInstance<T>);

    public static ImmutableArray<T> ToObjects<T>(List list, int index, Func<IValue, T> creator)
        where T : IBencodable
        => [.. ((List)list[index]).Select(creator)];

    public static ImmutableArray<Address> ToAddresses(List list, int index)
        => ToObjects(list, index, item => new Address(item));

    public static T ToEnum<T>(List list, int index)
        where T : Enum
        => (T)(object)(int)(Integer)list[index];

    private static T CreateInstance<T>(IValue value)
        where T : IBencodable
    {
        if (Activator.CreateInstance(typeof(T), value) is not T instance)
        {
            throw new InvalidCastException($"Failed to create an instance of {typeof(T)}.");
        }

        return instance;
    }
}
