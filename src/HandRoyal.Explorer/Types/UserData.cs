using System.Collections.Immutable;
using HandRoyal.Serialization;
using HandRoyal.States;
using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace HandRoyal.Explorer.Types;

public class UserData(User user, FungibleAssetValue balance)
{
    [Property(0)]
    public Address Id => user.Id;

    [Property(1)]
    public string Name => user.Name;

    [Property(2)]
    public ImmutableArray<Address> RegisteredGloves => user.RegisteredGloves;

    [Property(3)]
    public ImmutableArray<GloveInfo> OwnedGloves => user.OwnedGloves;

    [Property(4)]
    public Address EquippedGlove => user.EquippedGlove;

    [Property(5)]
    public Address SessionId => user.SessionId;

    [Property(6)]
    public long Balance => (long)balance.MajorUnit;
}
