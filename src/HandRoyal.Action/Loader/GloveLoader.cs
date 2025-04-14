#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
using HandRoyal.Game.Gloves;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Loader;

public static class GloveLoader
{
    private static readonly GloveFactory _factory;

    static GloveLoader()
    {
        _factory = new GloveFactory();
    }

    public static IGlove LoadGlove(Address id)
    {
        return _factory.CreateGlove(id.ToString());
    }

    public static string PickUpGlove(IRandom random, bool ensureUncommon)
    {
        return _factory.PickUpGlove(random, ensureUncommon);
    }
}
