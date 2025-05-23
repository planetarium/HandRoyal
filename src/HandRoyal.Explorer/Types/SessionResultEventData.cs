using GraphQL.AspNet.Attributes;
using HandRoyal.Enums;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

internal sealed class SessionResultEventData(Session session)
{
    [GraphSkip]
    public Session Session => session;

    public Address? SessionId => session.Metadata.Id;

    public Address[] WinnerIds => [.. session.UserEntries
        .Where(item => item.State == UserEntryState.Won)
        .Select(item => item.Id)];

    public Address[] LoserIds => [.. session.UserEntries
        .Where(item => item.State == UserEntryState.Lose)
        .Select(item => item.Id)];
}
