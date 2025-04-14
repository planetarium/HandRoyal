using Libplanet.Action;

namespace HandRoyal.Extensions;

public static class RandomExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IRandom random, IEnumerable<T> items)
    {
        var values = items.ToArray();
        var n = values.Length;

        for (var i = 0; i < n - 1; i++)
        {
            var j = random.Next(i, n);

            if (j != i)
            {
                (values[i], values[j]) = (values[j], values[i]);
            }
        }

        return values;
    }
}
