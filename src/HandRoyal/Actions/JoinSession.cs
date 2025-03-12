using HandRoyal.Exceptions;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
[Model(Version = 1)]
[GasUsage(1)]
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
        var user = (User)world[Addresses.Users, signer];
        var gloveId = Glove;

        if (!gloveId.Equals(default))
        {
            if (!world[Addresses.Gloves].TryGetValue<Glove>(gloveId, out _))
            {
                throw new JoinSessionException($"Glove with id {gloveId} not found");
            }

            if (gloveId != default && !user.OwnedGloves.Contains(gloveId))
            {
                throw new JoinSessionException(
                    $"Use {context.Signer} does not own glove {gloveId}");
            }
        }

        var height = context.BlockIndex;

        world[Addresses.Users, signer] =
            user with { SessionId = SessionId, EquippedGlove = gloveId };
        world[Addresses.Sessions, SessionId] = session.Join(height, user);
    }
}
