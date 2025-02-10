using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;
using Libplanet.Crypto;

namespace HandRoyal.Node.Explorer.Types;

public sealed class AddressScalarType()
    : ScalarGraphTypeBase("Address", typeof(Address))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return new Address(text);
    }
}
