using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;

namespace HandRoyal.Node.Explorer.Types;

public sealed class HexValueScalarType()
    : ScalarGraphTypeBase("Hex", typeof(HexValue))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return HexValue.Parse(text);
    }

    public override object Serialize(object item)
        => item is HexValue hexValue ? hexValue.ToString() : base.Serialize(item);

    public override bool ValidateObject(object item) => item is HexValue;
}
