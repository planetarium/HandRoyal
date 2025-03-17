#pragma warning disable S2094 // Classes should not be empty
using HandRoyal.Serialization;

namespace HandRoyal.Tests.Serialization;

public sealed partial class SerializerTest
{
    [Fact]
    public void ArrayState_Test()
    {
        var expectedState = new ArrayState
        {
            Values = [1, 2, 3],
        };
        var value = ModelSerializer.Serialize(expectedState);
        var actualState = ModelSerializer.Deserialize<ArrayState>(value);
        Assert.Equal(expectedState, actualState);
    }

    [Fact]
    public void InvalidArrayState_Test()
    {
        var expectedState = new InvalidArrayState
        {
            Values = [1, 2, 3],
        };
        Assert.Throws<ModelSerializationException>(
            () => ModelSerializer.Serialize(expectedState));
    }

    [Fact]
    public void State_Test()
    {
        var expectedState = new State
        {
            Value = 1,
        };
        var value = ModelSerializer.Serialize(expectedState);
        var actualState = ModelSerializer.Deserialize<State>(value);
        Assert.Equal(expectedState, actualState);
    }

    public abstract record class StateParent<T>
        where T : StateParent<T>
    {
    }

    [Model(Version = 1)]
    public sealed record class ArrayState : StateParent<ArrayState>, IEquatable<ArrayState>
    {
        [Property(0)]
        public int[] Values { get; set; } = [];

        public bool Equals(ArrayState? other) => ModelUtility.Equals(this, other);

        public override int GetHashCode() => ModelUtility.GetHashCode(this);
    }

    [Model(Version = 1)]
    public sealed record class InvalidArrayState : StateParent<ArrayState>
    {
        [Property(0)]
        public int[] Values { get; set; } = [];
    }

    [Model(Version = 1)]
    public sealed record class State : StateParent<State>
    {
        [Property(0)]
        public int Value { get; set; }
    }
}
