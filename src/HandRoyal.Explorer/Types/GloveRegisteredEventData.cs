using GraphQL.AspNet.Attributes;
using HandRoyal.Enums;
using HandRoyal.Gloves;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

public sealed class GloveRegisteredEventData(Glove glove)
{
    [GraphSkip]
    public Glove Glove => glove;

    public Address Id => Glove.Id;

    public GloveType Type => Glove.Type;
}
