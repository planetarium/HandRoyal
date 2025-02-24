using System.Numerics;
using HandRoyal.Serialization;
using static HandRoyal.Tests.RandomUtility;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    [Fact]
    public void ObjectRecordStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ObjectRecordStruct();
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ObjectRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ObjectRecordStruct_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectRecordStruct(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ObjectRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ObjectRecordStruct_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectRecordStruct(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ObjectRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void ArrayRecordStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ArrayRecordStruct();
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ArrayRecordStruct_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayRecordStruct(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ArrayRecordStruct_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayRecordStruct(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void MixedRecordStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new MixedRecordStruct();
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<MixedRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void MixedRecordStruct_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new MixedRecordStruct(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<MixedRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void MixedRecordStruct_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayRecordStruct(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayRecordStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Model(0)]
    public readonly record struct ObjectRecordStruct : IEquatable<ObjectRecordStruct>
    {
        [Property(0)]
        public int Int { get; init; }

        [Property(1)]
        public long Long { get; init; }

        [Property(2)]
        public BigInteger BigInteger { get; init; }

        [Property(3)]
        public TestEnum Enum { get; init; }

        [Property(4)]
        public bool Bool { get; init; }

        [Property(5)]
        public string String { get; init; } = string.Empty;

        [Property(6)]
        public DateTimeOffset DateTimeOffset { get; init; } = DateTimeOffset.UnixEpoch;

        [Property(7)]
        public TimeSpan TimeSpan { get; init; }

        public ObjectRecordStruct()
        {
        }

        public ObjectRecordStruct(Random random)
        {
            Int = Int32(random);
            Long = Int64(random);
            BigInteger = BigInteger(random);
            Enum = Enum<TestEnum>(random);
            Bool = Boolean(random);
            String = String(random);
            DateTimeOffset = DateTimeOffset(random);
            TimeSpan = TimeSpan(random);
        }

        public bool Equals(ObjectRecordStruct other) => SerializerUtility.Equals(this, other);

        public override int GetHashCode() => SerializerUtility.GetHashCode(this);
    }

    [Model(0)]
    public readonly record struct ArrayRecordStruct : IEquatable<ArrayRecordStruct>
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

        public ArrayRecordStruct()
        {
        }

        public ArrayRecordStruct(Random random)
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

        public bool Equals(ArrayRecordStruct other) => SerializerUtility.Equals(this, other);

        public override int GetHashCode() => SerializerUtility.GetHashCode(this);
    }

    [Model(0)]
    public readonly record struct MixedRecordStruct : IEquatable<MixedRecordStruct>
    {
        [Property(0)]
        public ObjectRecordStruct Object { get; init; }

        [Property(1)]
        public ObjectRecordStruct[] Objects { get; init; } = [];

        public MixedRecordStruct()
        {
            Object = new ObjectRecordStruct();
        }

        public MixedRecordStruct(Random random)
        {
            Object = new ObjectRecordStruct(random);
            Objects = Array(random, () => new ObjectRecordStruct(random));
        }

        public bool Equals(MixedRecordStruct other) => SerializerUtility.Equals(this, other);

        public override int GetHashCode() => SerializerUtility.GetHashCode(this);
    }
}
