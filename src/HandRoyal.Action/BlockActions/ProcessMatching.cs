﻿using System.Collections.Immutable;
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
            var userEntry1 = new UserEntry
            {
                Id = matching[0].UserId,
                InitialGloves = matching[0].Gloves,
            };
            var userEntry2 = new UserEntry
            {
                Id = matching[1].UserId,
                InitialGloves = matching[1].Gloves,
            };
            CreateSession(world, context.GetRandom(), userEntry1, userEntry2);
            matching = matching.RemoveRange(0, 2);
        }

        world[Addresses.MatchPool, Addresses.MatchPool] = matching;
    }

    private void CreateSession(
        IWorldContext world,
        IRandom random,
        UserEntry userEntry1,
        UserEntry userEntry2)
    {
        var buffer = ((Address)default).ToByteArray();
        random.NextBytes(buffer);
        var sessionId = new Address(buffer);
        var sessionMetadata = new SessionMetadata
        {
            Id = sessionId,
            Organizer = userEntry1.Id,
            Prize = null,
            MaximumUser = 2,
        };
        var userAccount = world[Addresses.Users];
        if (!userAccount.TryGetValue<User>(userEntry1.Id, out var user1))
        {
            return;
        }

        if (!userAccount.TryGetValue<User>(userEntry2.Id, out var user2))
        {
            return;
        }

        world[Addresses.Users, userEntry1.Id] = user1 with { SessionId = sessionId };
        world[Addresses.Users, userEntry2.Id] = user2 with { SessionId = sessionId };

        var sessionList = world.GetValue(Addresses.Sessions, Addresses.Sessions, List.Empty);
        world[Addresses.Sessions, Addresses.Sessions] = sessionList.Add(sessionId.Bencoded);
        world[Addresses.Sessions, sessionId] = new Session
            { Metadata = sessionMetadata, UserEntries = [userEntry1, userEntry2] };
    }
}
