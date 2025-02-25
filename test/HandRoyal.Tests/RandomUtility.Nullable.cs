namespace HandRoyal.Tests;

public static partial class RandomUtility
{
    public static T? Nullable<T>(Func<T> generator)
        where T : struct
    {
        if (Boolean() == true)
        {
            return generator();
        }

        return null;
    }

    public static T? NullableObject<T>(Func<T> generator)
        where T : class
    {
        if (Boolean() == true)
        {
            return generator();
        }

        return null;
    }

    public static T?[] NullableArray<T>(Func<T> generator)
        where T : struct
    {
        if (Boolean() == true)
        {
            return [];
        }

        var length = Length();
        var items = new T?[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = Nullable(generator);
        }

        return items;
    }

    public static T?[] NullableObjectArray<T>(Func<T> generator)
        where T : class
    {
        if (Boolean() == true)
        {
            return [];
        }

        var length = Length();
        var items = new T?[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = NullableObject(generator);
        }

        return items;
    }
}
