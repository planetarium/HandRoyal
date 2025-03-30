using System.Collections.Immutable;
using Bencodex.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.BlockActions;

internal sealed class ProcessMatching : BlockActionBase
{
    protected override void OnExecute(IWorldContext world, IActionContext context)
    {
        var matching = world.GetValue<ImmutableArray<MatchingInfo>>(
            Addresses.MatchPool,
            Addresses.MatchPool,
            []);
        if (matching.Length < 2)
        {
            return;
        }

        // 앞에서부터 둘씩 짝지어서 세션 생성
        while (matching.Length > 1)
        {
            var player1 = new Player { Id = matching[0].UserId, Gloves = matching[0].Gloves };
            var player2 = new Player { Id = matching[1].UserId, Gloves = matching[1].Gloves };
            CreateSession(world, context.GetRandom(), player1, player2);
            matching = matching.RemoveRange(0, 2);
        }

        world[Addresses.MatchPool, Addresses.MatchPool] = matching;
    }

    private void CreateSession(
        IWorldContext world,
        IRandom random,
        Player player1,
        Player player2)
    {
        var buffer = ((Address)default).ToByteArray();
        random.NextBytes(buffer);
        var sessionId = new Address(buffer);
        var sessionMetadata = new SessionMetadata
        {
            Id = sessionId,
            Organizer = player1.Id,
            Prize = default,
            MaximumUser = 2,
        };
        var sessionList = world.GetValue(Addresses.Sessions, Addresses.Sessions, List.Empty);
        world[Addresses.Sessions, Addresses.Sessions] = sessionList.Add(sessionId.Bencoded);
        world[Addresses.Sessions, sessionId] = new Session
            { Metadata = sessionMetadata, Players = [player1, player2] };
    }
}
