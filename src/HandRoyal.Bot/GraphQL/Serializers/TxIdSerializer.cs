using Libplanet.Types.Tx;
using StrawberryShake.Serialization;

namespace HandRoyal.Bot.GraphQL.Serializers;

public sealed class TxIdSerializer : ScalarSerializer<string, TxId>
{
    public TxIdSerializer()
        : base(nameof(TxId))
    {
    }

    public override TxId Parse(string serializedValue) => TxId.FromHex(serializedValue);

    protected override string Format(TxId runtimeValue) => runtimeValue.ToHex();
}
