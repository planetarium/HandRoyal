using System.Numerics;
using HandRoyal.Serialization;
using static HandRoyal.Tests.RandomUtility;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    [Fact]
    public void ObjectStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ObjectStruct();
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ObjectStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ObjectStruct_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectStruct(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ObjectStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ObjectStruct_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectStruct(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ObjectStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void ArrayStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ArrayStruct();
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ArrayStruct_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayStruct(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ArrayStruct_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayStruct(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void MixedStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new MixedStruct();
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<MixedStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void MixedStruct_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new MixedStruct(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<MixedStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void MixedStruct_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayStruct(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Model(Version = 1)]
    public readonly struct ObjectStruct : IEquatable<ObjectStruct>
    {
        [Property(0)]
        public int Int { get; init; }

        [Property(1)]
        public long Long { get; init; }

        [Property(2)]
        public BigInteger BigInteger { get; init; }

        [Property(3)]
        public byte[] Bytes { get; init; } = [];

        [Property(4)]
        public TestEnum Enum { get; init; }

        [Property(5)]
        public bool Bool { get; init; }

        [Property(6)]
        public string String { get; init; } = string.Empty;

        [Property(7)]
        public DateTimeOffset DateTimeOffset { get; init; } = DateTimeOffset.UnixEpoch;

        [Property(8)]
        public TimeSpan TimeSpan { get; init; }

        public ObjectStruct()
        {
        }

        public ObjectStruct(Random random)
        {
            Int = Int32(random);
            Long = Int64(random);
            BigInteger = BigInteger(random);
            Bytes = Array(random, Byte);
            Enum = Enum<TestEnum>(random);
            Bool = Boolean(random);
            String = String(random);
            DateTimeOffset = DateTimeOffset(random);
            TimeSpan = TimeSpan(random);
        }

        public bool Equals(ObjectStruct other) => ModelUtility.Equals(this, other);

        public override bool Equals(object? obj) => obj is ObjectStruct @struct && Equals(@struct);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }

    [Model(Version = 1)]
    public readonly struct ArrayStruct : IEquatable<ArrayStruct>
    {
        [Property(0)]
        public int[] Ints { get; init; } = [];

        [Property(1)]
        public long[] Longs { get; init; } = [];

        [Property(2)]
        public BigInteger[] BigIntegers { get; init; } = [];

        [Property(3)]
        public TestEnum[] Enums { get; init; } = [];

        [Property(4)]
        public bool[] Bools { get; init; } = [];

        [Property(5)]
        public string[] Strings { get; init; } = [];

        [Property(6)]
        public DateTimeOffset[] DateTimeOffsets { get; init; } = [];

        [Property(7)]
        public TimeSpan[] TimeSpans { get; init; } = [];

        public ArrayStruct()
        {
        }

        public ArrayStruct(Random random)
        {
            Ints = Array(random, Int32);
            Longs = Array(random, Int64);
            BigIntegers = Array(random, BigInteger);
            Enums = Array(random, Enum<TestEnum>);
            Bools = Array(random, Boolean);
            Strings = Array(random, String);
            DateTimeOffsets = Array(random, DateTimeOffset);
            TimeSpans = Array(random, TimeSpan);
        }

        public bool Equals(ArrayStruct other) => ModelUtility.Equals(this, other);

        public override bool Equals(object? obj) => obj is ArrayStruct @struct && Equals(@struct);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }

    [Model(Version = 1)]
    public readonly struct MixedStruct : IEquatable<MixedStruct>
    {
        [Property(0)]
        public ObjectStruct Object { get; init; }

        [Property(1)]
        public ObjectStruct[] Objects { get; init; } = [];

        public MixedStruct()
        {
            Object = new ObjectStruct();
        }

        public MixedStruct(Random random)
        {
            Object = new ObjectStruct(random);
            Objects = Array(random, () => new ObjectStruct(random));
        }

        public bool Equals(MixedStruct other) => ModelUtility.Equals(this, other);

        public override bool Equals(object? obj) => obj is MixedStruct @struct && Equals(@struct);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }
}
