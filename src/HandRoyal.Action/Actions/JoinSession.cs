using System.Collections.Immutable;
using HandRoyal.Exceptions;
using HandRoyal.Loader;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("JoinSession")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class JoinSession : ActionBase, IEquatable<JoinSession>
{
    public const int JoinReward = 2;

    [Property(0)]
    public required Address SessionId { get; init; }

    [Property(1)]
    public required ImmutableArray<Address> Gloves { get; init; }

    public bool Equals(JoinSession? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        var session = (Session)world[Addresses.Sessions, SessionId];
        var user = (User)world[Addresses.Users, signer];
        var gloves = Gloves;

        if (gloves.Length != session.Metadata.NumberOfInitialGloves)
        {
            throw new JoinSessionException("Gloves must have same number of rounds.");
        }

        foreach (var glove in gloves)
        {
            _ = GloveLoader.LoadGlove(glove);
            var count = user.OwnedGloves.FirstOrDefault(info => info.Id.Equals(glove))?.Count ?? 0;
            if (count == 0)
            {
                throw new JoinSessionException($"User {signer} does not own the glove {glove}");
            }

            if (gloves.Count(g => g.Equals(glove)) > count)
            {
                throw new JoinSessionException(
                    $"User {signer} does not own enough number of glove {glove}");
            }
        }

        var height = context.BlockIndex;

        world.TransferAsset(Currencies.SinkAddress, context.Signer, Currencies.Royal * JoinReward);
        user = user.DecreaseActionPoint(1);
        world[Addresses.Users, signer] = user with { SessionId = SessionId };
        world[Addresses.Sessions, SessionId] = session.Join(height, user, gloves);
    }
}
