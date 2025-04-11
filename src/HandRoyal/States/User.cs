using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using HandRoyal.Serialization;
using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class User : IEquatable<User>
{
    public const int MaxRefillAp = 15;
    public const int ClaimInterval = 10_000;

    [Property(0)]
    public required Address Id { get; init; }

    [Property(1)]
    public required string Name { get; init; }

    [Property(2)]
    public ImmutableArray<Address> RegisteredGloves { get; init; } = [];

    [Property(3)]
    public required ImmutableArray<GloveInfo> OwnedGloves { get; init; }

    [Property(4)]
    public Address EquippedGlove { get; init; }

    [Property(5)]
    public Address SessionId { get; init; }

    [Property(6)]
    public int ActionPoint { get; init; }

    [Property(7)]
    public long LastClaimedAt { get; init; }

    public static User GetUser(IWorldContext world, Address userId)
        => (User)world[Addresses.Users, userId];

    public static bool TryGetUser(
        IWorldContext world, Address userId, [MaybeNullWhen(false)] out User user)
        => world.TryGetValue(Addresses.Users, userId, out user);

    [Pure]
    public User ObtainGlove(Address glove, int count)
    {
        var index = -1;
        for (int i = 0; i < OwnedGloves.Length; i++)
        {
            if (OwnedGloves[i].Id.Equals(glove))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            if (count <= 0)
            {
                return this;
            }

            return this with
            {
                OwnedGloves = OwnedGloves.Add(new GloveInfo { Id = glove, Count = count }),
            };
        }

        var nextCount = OwnedGloves[index].Count + count;
        nextCount = nextCount < 0 ? 0 : nextCount;

        return this with
        {
            OwnedGloves = OwnedGloves
                .RemoveAt(index)
                .Add(new GloveInfo { Id = glove, Count = nextCount }),
        };
    }

    [Pure]
    public User RefillActionPoint(long blockIndex)
    {
        if (blockIndex - (blockIndex % ClaimInterval) > LastClaimedAt)
        {
            return this with
            {
                ActionPoint = MaxRefillAp,
                LastClaimedAt = blockIndex,
            };
        }

        return this;
    }

    public bool Equals(User? other) => ModelUtility.Equals(this, other);

    public override int GetHashCode() => ModelUtility.GetHashCode(this);
}
