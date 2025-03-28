using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.Exceptions;
using HandRoyal.Extensions;
using HandRoyal.Gloves;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("CreateSession")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class CreateSession : ActionBase, IEquatable<CreateSession>
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

    [Property(9)]
    public int InitialHealthPoint { get; init; } = SessionMetadata.Default.InitialHealthPoint;

    [Property(10)]
    public int NumberOfGloves { get; init; } = SessionMetadata.Default.NumberOfGloves;

    [Property(11)]
    public ImmutableArray<Address> Users { get; init; } = SessionMetadata.Default.Users;

    public bool Equals(CreateSession? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        if (SessionId == default)
        {
            throw new CreateSessionException("Session id is not set");
        }

        if (NumberOfGloves < MaxRounds)
        {
            throw new CreateSessionException("NumberOfGloves must be greater than MaxRounds.");
        }

        if (world.Contains(Addresses.Sessions, SessionId))
        {
            throw new CreateSessionException($"Session of id {SessionId} already exists");
        }

        var signer = context.Signer;

        try
        {
            _ = GloveLoader.LoadGlove(Prize);
        }
        catch (Exception e)
        {
            throw new CreateSessionException($"Glove of id {Prize} does not exist", e);
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
            InitialHealthPoint = InitialHealthPoint,
            NumberOfGloves = NumberOfGloves,
            Users = Users,
        };
        var sessionList = world.GetValue(Addresses.Sessions, Addresses.Sessions, List.Empty);
        world[Addresses.Sessions, Addresses.Sessions] = sessionList.Add(SessionId);
        world[Addresses.Sessions, SessionId] = new Session { Metadata = sessionMetadata };
    }
}
