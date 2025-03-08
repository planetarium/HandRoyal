using GraphQL.AspNet.Attributes;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

public sealed class GloveRegisteredEventData(Glove glove)
{
    [GraphSkip]
    public Glove Glove => glove;

    public Address Id => Glove.Id;

    public Address Author => Glove.Author;
}
