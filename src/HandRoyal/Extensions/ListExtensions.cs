using Bencodex;
using Bencodex.Types;

namespace HandRoyal.Extensions;

public static class ListExtensions
{
    public static List Add(this List @this, IBencodable item) => @this.Add(item.Bencoded);
}
