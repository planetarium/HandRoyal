using Libplanet.Crypto;

namespace HandRoyal.Gloves;

public static class GloveLoader
{
    private static readonly GloveFactory _factory = new("src/HandRoyal/Gloves/Data/gloves.json");

    public static IGlove LoadGlove(Address id)
    {
        return _factory.CreateGlove(id.ToString());
    }
}
