using System.Collections.Immutable;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Bot;

public static class GloveUtility
{
    public static bool IsRock(this Address gloveId) => gloveId.ToString().StartsWith("0x0");

    public static bool IsPaper(this Address gloveId) => gloveId.ToString().StartsWith("0x1");

    public static bool IsScissors(this Address gloveId) => gloveId.ToString().StartsWith("0x2");

    public static Address[] GetRandomGloves(ImmutableArray<GloveInfo> gloveInfos, int count)
        => GetRandomGloves([.. GetGloves(gloveInfos)], count);

    public static Address[] GetRandomGloves(Address[] gloveIds, int count)
    {
        var gloveList = new List<Address>(count);
        var order = Enumerable.Range(0, 3).ToArray();
        var items = new Address[][]
        {
            Shuffle([.. gloveIds.Where(IsRock)]),
            Shuffle([.. gloveIds.Where(IsScissors)]),
            Shuffle([.. gloveIds.Where(IsPaper)]),
        };
        var indexes = new int[3];
        var type = 0;
        var totalCount = gloveIds.Length;

        Random.Shared.Shuffle(order);
        while (gloveList.Count < count && gloveList.Count < totalCount)
        {
            var i = indexes[type];
            if (i < items[type].Length)
            {
                gloveList.Add(items[type][i]);
                indexes[type]++;
            }

            type++;
            type %= 3;
        }

        return [.. gloveList];
    }

    public static IEnumerable<Address> GetGloves(ImmutableArray<GloveInfo> gloveInfos)
    {
        foreach (var gloveInfo in gloveInfos)
        {
            for (var i = 0; i < gloveInfo.Count; i++)
            {
                yield return gloveInfo.Id;
            }
        }
    }

    private static Address[] Shuffle(Address[] items)
    {
        Random.Shared.Shuffle(items);
        return items;
    }
}
