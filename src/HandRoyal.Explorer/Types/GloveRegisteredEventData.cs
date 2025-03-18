using GraphQL.AspNet.Attributes;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

public sealed class GloveRegisteredEventData(IGlove glove)
{
    [GraphSkip]
    public IGlove Glove => glove;

    public Address Id => Glove.Id;

    public GloveType Type => Glove.Type;
}
