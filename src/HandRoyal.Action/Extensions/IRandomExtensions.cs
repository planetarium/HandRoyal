using Libplanet.Action;

namespace HandRoyal.Extensions;

public static class IRandomExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IRandom @this, IEnumerable<T> items)
    {
        var random = new Random(@this.Seed);
        var values = items.ToArray();
        random.Shuffle(values);

        return values;
    }
}
