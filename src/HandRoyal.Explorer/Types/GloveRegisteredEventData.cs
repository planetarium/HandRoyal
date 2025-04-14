using GraphQL.AspNet.Attributes;
using HandRoyal.Game.Gloves;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

public sealed class GloveRegisteredEventData(IGlove glove)
{
    [GraphSkip]
    public IGlove Glove => glove;

    public Address Id => Glove.Id;

    public GloveType Type => Glove.Type;
}
