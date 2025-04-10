using System.Collections.Immutable;
using HandRoyal.Exceptions;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Action;

namespace HandRoyal.Actions;

[ActionType("CancelMatching")]
[Model(Version = 1)]
[GasUsage(1)]
public sealed record class CancelMatching : ActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var signer = context.Signer;
        if (!world.TryGetValue<User>(Addresses.Users, signer, out _))
        {
            throw new CancelMatchingException($"User of id {signer} does not exist.");
        }

        var matchPool = world.GetValue<ImmutableArray<MatchingInfo>>(
            Addresses.MatchPool,
            Addresses.MatchPool,
            []);
        if (!matchPool.Any(info => info.UserId.Equals(signer)))
        {
            throw new CancelMatchingException(
                $"User of id {signer} does not exist in the matching pool.");
        }

        matchPool = matchPool.Remove(matchPool.First(info => info.UserId.Equals(signer)));
        world[Addresses.MatchPool, Addresses.MatchPool] = matchPool;
    }
}
