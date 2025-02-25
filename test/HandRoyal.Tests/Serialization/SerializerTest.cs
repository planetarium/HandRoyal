using System.Collections.Immutable;
using System.Numerics;
using HandRoyal.Serialization;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    public static IEnumerable<object[]> RandomSeeds =>
    [
        [Random.Shared.Next()],
        [Random.Shared.Next()],
        [Random.Shared.Next()],
        [Random.Shared.Next()],
    ];

    [Theory]
    [InlineData(typeof(bool))]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    [InlineData(typeof(BigInteger))]
    [InlineData(typeof(byte[]))]
    [InlineData(typeof(TestEnum))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(TimeSpan))]
    public void CanSupport_Test(Type type)
    {
        Assert.True(ModelSerializer.CanSupportType(type));
    }

    [Theory]
    [InlineData(typeof(bool[]))]
    [InlineData(typeof(string[]))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(long[]))]
    [InlineData(typeof(BigInteger[]))]
    [InlineData(typeof(byte[][]))]
    [InlineData(typeof(TestEnum[]))]
    [InlineData(typeof(DateTimeOffset[]))]
    [InlineData(typeof(TimeSpan[]))]
    public void CanSupportArray_Test(Type type)
    {
        Assert.True(ModelSerializer.CanSupportType(type));
    }

    [Theory]
    [InlineData(typeof(ImmutableArray<bool>))]
    [InlineData(typeof(ImmutableArray<string>))]
    [InlineData(typeof(ImmutableArray<int>))]
    [InlineData(typeof(ImmutableArray<long>))]
    [InlineData(typeof(ImmutableArray<BigInteger>))]
    [InlineData(typeof(ImmutableArray<byte[]>))]
    [InlineData(typeof(ImmutableArray<TestEnum>))]
    [InlineData(typeof(ImmutableArray<DateTimeOffset>))]
    [InlineData(typeof(ImmutableArray<TimeSpan>))]
    public void CanSupportImmutableArray_Test(Type type)
    {
        Assert.True(ModelSerializer.CanSupportType(type));
    }

    [Fact]
    public void CanSupportNonSerializable_FailTest()
    {
        Assert.False(ModelSerializer.CanSupportType(typeof(object)));
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Dictionary<string, string>))]
    [InlineData(typeof(List<string>))]
    [InlineData(typeof(HashSet<string>))]
    [InlineData(typeof(Stack<string>))]
    [InlineData(typeof(Queue<string>))]
    [InlineData(typeof(ImmutableList<string>))]
    [InlineData(typeof(ImmutableHashSet<string>))]
    [InlineData(typeof(ImmutableDictionary<string, string>))]
    [InlineData(typeof(ImmutableQueue<string>))]
    [InlineData(typeof(ImmutableStack<string>))]
    public void CanSupport_FailTest(Type type)
    {
        Assert.False(ModelSerializer.CanSupportType(type));
    }

    public enum TestEnum
    {
        A,
        B,
        C,
    }
}
