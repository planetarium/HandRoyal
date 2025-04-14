using System.Collections.Immutable;
using HandRoyal.Exceptions;
using HandRoyal.Loader;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Actions;

[ActionType("RegisterMatching")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class RegisterMatching : ActionBase, IEquatable<RegisterMatching>
{
    [Property(0)]
    public required ImmutableArray<Address> Gloves { get; init; }

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    public bool Equals(RegisterMatching? other) => ModelUtility.Equals(this, other);

    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        if (!world.TryGetValue<User>(Addresses.Users, signer, out var user))
        {
            throw new RegisterMatchingException($"User of id {signer} does not exist.");
        }

        var matchPool = world.GetValue<ImmutableArray<MatchingInfo>>(
            Addresses.MatchPool,
            Addresses.MatchPool,
            []);
        if (matchPool.Any(info => info.UserId.Equals(signer)))
        {
            throw new RegisterMatchingException(
                $"User of id {signer} already exists in the matching pool.");
        }

        var gloves = Gloves;
        if (gloves.Length != SessionMetadata.Default.NumberOfInitialGloves)
        {
            throw new RegisterMatchingException("Gloves number mismatch.");
        }

        foreach (var glove in gloves)
        {
            _ = GloveLoader.LoadGlove(glove);
            var count = user.OwnedGloves.FirstOrDefault(info => info.Id.Equals(glove))?.Count ?? 0;
            if (count == 0)
            {
                throw new RegisterMatchingException(
                    $"User {signer} does not own the glove {glove}");
            }

            if (gloves.Count(g => g.Equals(glove)) > count)
            {
                throw new RegisterMatchingException(
                    $"User {signer} does not own enough number of glove {glove}");
            }
        }

        matchPool = matchPool.Add(
            new MatchingInfo
            {
                UserId = signer,
                Gloves = gloves,
                RegisteredHeight = context.BlockIndex,
            });
        world[Addresses.Users, signer] = user.DecreaseActionPoint(1);
        world[Addresses.MatchPool, Addresses.MatchPool] = matchPool;
    }
}
