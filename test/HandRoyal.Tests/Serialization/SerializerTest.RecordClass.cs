using System.Numerics;
using HandRoyal.Serialization;
using static HandRoyal.Tests.RandomUtility;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    [Fact]
    public void ObjectRecordClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ObjectRecordClass();
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ObjectRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ObjectRecordClass_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectRecordClass(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ObjectRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ObjectRecordClass_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectRecordClass(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ObjectRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void ArrayRecordClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ArrayRecordClass();
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ArrayRecordClass_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayRecordClass(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ArrayRecordClass_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayRecordClass(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void MixedRecordClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new MixedRecordClass();
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<MixedRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void MixedRecordClass_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new MixedRecordClass(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<MixedRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void MixedRecordClass_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayRecordClass(random);
        var serialized = ModelSerializer.Serialize(expectedObject);
        var actualObject = ModelSerializer.Deserialize<ArrayRecordClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Model(Version = 1)]
    public sealed record class ObjectRecordClass : IEquatable<ObjectRecordClass>
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

        public ObjectRecordClass()
        {
        }

        public ObjectRecordClass(Random random)
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

        public bool Equals(ObjectRecordClass? other) => ModelUtility.Equals(this, other);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }

    [Model(Version = 1)]
    public sealed record class ArrayRecordClass : IEquatable<ArrayRecordClass>
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

        public ArrayRecordClass()
        {
        }

        public ArrayRecordClass(Random random)
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

        public bool Equals(ArrayRecordClass? other) => ModelUtility.Equals(this, other);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }

    [Model(Version = 1)]
    public sealed record class MixedRecordClass : IEquatable<MixedRecordClass>
    {
        [Property(0)]
        public ObjectRecordClass Object { get; init; }

        [Property(1)]
        public ObjectRecordClass[] Objects { get; init; } = [];

        public MixedRecordClass()
        {
            Object = new ObjectRecordClass();
        }

        public MixedRecordClass(Random random)
        {
            Object = new ObjectRecordClass(random);
            Objects = Array(random, () => new ObjectRecordClass(random));
        }

        public bool Equals(MixedRecordClass? other) => ModelUtility.Equals(this, other);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }
}
