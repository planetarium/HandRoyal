using GraphQL.AspNet.Attributes;
using HandRoyal.States;

namespace HandRoyal.Explorer.Types;

internal sealed class SessionEventData(Session session)
{
    [GraphSkip]
    public Session Session => session;

    public long Height => session.Height;

    public SessionState State => session.State;

    public Match? Match { get; set; }
}
