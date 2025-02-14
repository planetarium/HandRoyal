using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

internal sealed record class SubmitMoveEventData
{
    public Address SessionId { get; init; }

    public Address UserId { get; init; }

    public MoveType Move { get; init; }
}
