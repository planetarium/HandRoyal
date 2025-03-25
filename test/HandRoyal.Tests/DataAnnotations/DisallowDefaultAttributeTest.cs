using System.ComponentModel.DataAnnotations;
using HandRoyal.DataAnnotations;
using Libplanet.Crypto;

namespace HandRoyal.Tests.DataAnnotations;

public sealed class DisallowDefaultAttributeTest
{
    public static IEnumerable<object[]> Addresses =>
    [
        [default(Address), false],
        [new Address("0000000000000000000000000000000000000000"), false],
        [new Address("0000000000000000000000000000000000000001"), true],
    ];

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(-1, true)]
    public void Int_Test(int value, bool isValid)
    {
        var obj = new IntClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void Address_Test(Address address, bool isValid)
    {
        var obj = new AddressClass { Value = address };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Fact]
    public void Object_Test()
    {
        var obj = new ObjectClass { Value = null };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.True(result);
    }

    public sealed class IntClass
    {
        [DisallowDefault]
        public int Value { get; set; }
    }

    public sealed class AddressClass
    {
        [DisallowDefault]
        public Address Value { get; set; }
    }

    public sealed class ObjectClass
    {
        [DisallowDefault]
        public object? Value { get; set; }
    }
}
