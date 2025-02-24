using System.Numerics;
using HandRoyal.Serialization;
using static HandRoyal.Tests.RandomUtility;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    [Fact]
    public void ObjectClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ObjectClass();
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ObjectClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ObjectClass_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectClass(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ObjectClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ObjectClass_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectClass(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ObjectClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void ArrayClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ArrayClass();
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void ArrayClass_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayClass(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void ArrayClass_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayClass(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void MixedClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new MixedClass();
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<MixedClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    public void MixedClass_SerializeAndDeserialize_StaticSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new MixedClass(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<MixedClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [MemberData(nameof(RandomSeeds))]
    public void MixedClass_SerializeAndDeserialize_RandomSeed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayClass(random);
        var serialized = Serializer.Serialize(expectedObject);
        var actualObject = Serializer.Deserialize<ArrayClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Model(0)]
    public sealed class ObjectClass : IEquatable<ObjectClass>
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

        public ObjectClass()
        {
        }

        public ObjectClass(Random random)
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

        public bool Equals(ObjectClass? other) => SerializerUtility.Equals(this, other);

        public override bool Equals(object? obj) => Equals(obj as ObjectClass);

        public override int GetHashCode() => SerializerUtility.GetHashCode(this);
    }

    [Model(0)]
    public sealed class ArrayClass : IEquatable<ArrayClass>
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

        public ArrayClass()
        {
        }

        public ArrayClass(Random random)
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

        public bool Equals(ArrayClass? other) => SerializerUtility.Equals(this, other);

        public override bool Equals(object? obj) => Equals(obj as ArrayClass);

        public override int GetHashCode() => SerializerUtility.GetHashCode(this);
    }

    [Model(0)]
    public sealed class MixedClass : IEquatable<MixedClass>
    {
        [Property(0)]
        public ObjectClass Object { get; init; }

        [Property(1)]
        public ObjectClass[] Objects { get; init; } = [];

        public MixedClass()
        {
            Object = new ObjectClass();
        }

        public MixedClass(Random random)
        {
            Object = new ObjectClass(random);
            Objects = Array(random, () => new ObjectClass(random));
        }

        public bool Equals(MixedClass? other) => SerializerUtility.Equals(this, other);

        public override bool Equals(object? obj) => Equals(obj as MixedClass);

        public override int GetHashCode() => SerializerUtility.GetHashCode(this);
    }
}
