using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;

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
