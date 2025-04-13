#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
using System.Reflection;
using HandRoyal.Game.Gloves;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Loader;

public static class GloveLoader
{
    private static readonly GloveFactory _factory;

    static GloveLoader()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream =
            assembly.GetManifestResourceStream("HandRoyal.Action.Gloves.Data.gloves.json");
        if (stream == null)
        {
            throw new InvalidOperationException("Embedded resource 'gloves.json' not found.");
        }

        using var bufferedStream = new BufferedStream(stream);
        using var reader = new StreamReader(bufferedStream);
        var jsonContent = reader.ReadToEnd();
        _factory = new GloveFactory(jsonContent);
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
