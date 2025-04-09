using Libplanet.Crypto;
using StrawberryShake.Serialization;

namespace HandRoyal.Bot.GraphQL.Serializers;

public sealed class AddressSerializer : ScalarSerializer<string, Address>
{
    public AddressSerializer()
        : base(nameof(Address))
    {
    }

    public override Address Parse(string serializedValue) => new(serializedValue);

    protected override string Format(Address runtimeValue) => runtimeValue.ToHex();
}
