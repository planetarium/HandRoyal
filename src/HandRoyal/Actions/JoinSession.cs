using System.Collections.Immutable;
using HandRoyal.Exceptions;
using HandRoyal.Gloves;
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
    public required ImmutableArray<Address> Gloves { get; init; }

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        var session = (Session)world[Addresses.Sessions, SessionId];
        var user = (User)world[Addresses.Users, signer];
        var gloves = Gloves;

        foreach (var glove in gloves)
        {
            _ = GloveLoader.LoadGlove(glove);
            if (!user.OwnedGloves.TryGetValue(glove, out var ownedGlove))
            {
                throw new JoinSessionException($"User {signer} does not own the glove {glove}");
            }

            if (gloves.Count(g => g.Equals(glove)) > ownedGlove)
            {
                throw new JoinSessionException(
                    $"User {signer} does not own enough number of glove {glove}");
            }
        }

        var height = context.BlockIndex;

        world[Addresses.Users, signer] = user with { SessionId = SessionId };
        world[Addresses.Sessions, SessionId] = session.Join(height, user, gloves);
    }
}
