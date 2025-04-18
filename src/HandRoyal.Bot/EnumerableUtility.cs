using System.Collections.Immutable;

namespace HandRoyal.Bot;

public static class EnumerableUtility
{
    public static ImmutableArray<T> SelectMany<TSource, T>(
        IReadOnlyList<TSource?>? source, Func<TSource, T> selector)
    {
        if (source is null)
        {
            return [];
        }

        var builder = ImmutableArray.CreateBuilder<T>(source.Count);
        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            builder.Add(selector(item));
        }

        return builder.ToImmutable();
    }
}
