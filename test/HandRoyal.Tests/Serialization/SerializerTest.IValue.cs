using Bencodex.Types;
using HandRoyal.Serialization;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    public static IEnumerable<object[]> BencodexValues =>
    [
        [Null.Value],
        [new Bencodex.Types.Boolean(true)],
        [new Integer(0)],
        [new Text("Hello, World!")],
        [new List(Null.Value, new Integer(0), new Text("Hello, World!"))],
        [new Dictionary(
            [
                new KeyValuePair<IKey, IValue>(new Text("Null"), Null.Value),
                new KeyValuePair<IKey, IValue>(new Text("Integer"), new Integer(0)),
                new KeyValuePair<IKey, IValue>(new Text("Text"), new Text("Hello, World!")),
            ])],
    ];

    [Theory]
    [InlineData(typeof(Null))]
    [InlineData(typeof(Bencodex.Types.Boolean))]
    [InlineData(typeof(Integer))]
    [InlineData(typeof(Text))]
    [InlineData(typeof(List))]
    [InlineData(typeof(Dictionary))]
    public void CanSupport_IValueType_Test(Type type)
    {
        Assert.True(ModelSerializer.CanSupportType(type));
    }

    [Fact]
    public void CannotSupport_IValueType_Test()
    {
        Assert.False(ModelSerializer.CanSupportType(typeof(IValue)));
    }

    [Theory]
    [MemberData(nameof(BencodexValues))]
    public void IValueType_SerializeAndDeserialize_Test(IValue value)
    {
        var serialized = ModelSerializer.Serialize(value);
        var actualValue = ModelSerializer.Deserialize(serialized, value.GetType())!;
        Assert.Equal(value, actualValue);
    }
}
