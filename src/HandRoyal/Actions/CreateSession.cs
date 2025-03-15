using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.Extensions;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("CreateSession")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class CreateSession : ActionBase
{
    [Property(0)]
    public required Address SessionId { get; init; }

    [Property(1)]
    public required Address Prize { get; init; }

    [Property(2)]
    public int MaximumUser { get; init; } = SessionMetadata.Default.MaximumUser;

    [Property(3)]
    public int MinimumUser { get; init; } = SessionMetadata.Default.MinimumUser;

    [Property(4)]
    public int RemainingUser { get; init; } = SessionMetadata.Default.MaximumUser;

    [Property(5)]
    public long StartAfter { get; init; } = SessionMetadata.Default.StartAfter;

    [Property(6)]
    public int MaxRounds { get; init; } = SessionMetadata.Default.MaxRounds;

    [Property(7)]
    public long RoundLength { get; init; } = SessionMetadata.Default.RoundLength;

    [Property(8)]
    public long RoundInterval { get; init; } = SessionMetadata.Default.RoundInterval;

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (SessionId == default)
        {
            throw new CreateSessionException("Session id is not set");
        }

        if (world.Contains(Addresses.Sessions, SessionId))
        {
            throw new CreateSessionException($"Session of id {SessionId} already exists");
        }

        var signer = context.Signer;

        // Everyone can create session with default glove
        if (!Prize.Equals(default))
        {
            var glove = (Glove)world[Addresses.Gloves, Prize];
            if (!glove.Author.Equals(signer))
            {
                throw new CreateSessionException(
                    $"Organizer for session id {SessionId} is not author of the prize {Prize}");
            }
        }

        var sessionMetadata = new SessionMetadata
        {
            Id = SessionId,
            Organizer = signer,
            Prize = Prize,
            MaximumUser = MaximumUser,
            MinimumUser = MinimumUser,
            RemainingUser = RemainingUser,
            StartAfter = StartAfter,
            MaxRounds = MaxRounds,
            RoundLength = RoundLength,
            RoundInterval = RoundInterval,
        };
        var sessionList = world.GetValue(Addresses.Sessions, Addresses.Sessions, List.Empty);
        world[Addresses.Sessions, Addresses.Sessions] = sessionList.Add(SessionId);
        world[Addresses.Sessions, SessionId] = new Session { Metadata = sessionMetadata };
    }
}
