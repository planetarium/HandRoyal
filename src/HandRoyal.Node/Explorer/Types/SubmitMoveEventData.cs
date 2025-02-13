using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Node.Explorer.Types;

public sealed record class SubmitMoveEventData
{
    public Address SessionId { get; init; }

    public Address UserId { get; init; }

    public MoveType Move { get; init; }
}
