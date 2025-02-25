using HandRoyal.Exceptions;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
[Model(Version = 1)]
public sealed record class JoinSession : ActionBase
{
    [Property(0)]
    public required Address SessionId { get; init; }

    [Property(1)]
    public required Address Glove { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        var session = (Session)world[Addresses.Sessions, SessionId];
        var sessionMetadata = session.Metadata;
        if (session.State != SessionState.Ready)
        {
            var message =
                $"State of the session of id {SessionId} is not READY. " +
                $"(state: {session.State})";
            throw new JoinSessionException(message);
        }

        if (session.Players.Length >= sessionMetadata.MaximumUser)
        {
            var message =
                $"Participant registration of session of id {SessionId} is closed " +
                $"since max user count {sessionMetadata.MinimumUser} has reached.";
            throw new JoinSessionException(message);
        }

        if (session.FindPlayer(signer) != -1)
        {
            var message = $"Duplicated participation is prohibited. ({signer})";
            throw new JoinSessionException(message);
        }

        var user = (User)world[Addresses.Users, signer];
        if (user.SessionId != default)
        {
            throw new JoinSessionException("User is already in a session.");
        }

        if (Glove != default && !user.Gloves.Contains(Glove))
        {
            var message = $"Cannot join session with invalid glove {Glove}.";
            throw new JoinSessionException(message);
        }

        var player = new Player { Id = signer, Glove = Glove };
        var players = session.Players.Add(player);
        world[Addresses.Sessions, SessionId] = session with { Players = players };
        world[Addresses.Users, signer] = user with { SessionId = SessionId };
    }
}
