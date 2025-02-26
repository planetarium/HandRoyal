using System.ComponentModel.DataAnnotations;
using System.Numerics;
using HandRoyal.DataAnnotations;

namespace HandRoyal.Tests.DataAnnotations;

public sealed class NonPositiveAttributeTest
{
    [Theory]
    [InlineData("-1234567890123456789012345678901234567890", true)]
    [InlineData("0", true)]
    [InlineData("1234567890123456789012345678901234567890", false)]
    public void BigInteger_Test(string value, bool isValid)
    {
        var bigInteger = BigInteger.Parse(value);
        var obj = new BigIntegerClass { Value = bigInteger };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData((sbyte)-1, true)]
    [InlineData((sbyte)0, true)]
    [InlineData((sbyte)1, false)]
    public void SByte_Test(sbyte value, bool isValid)
    {
        var obj = new SByteClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData((short)-1, true)]
    [InlineData((short)0, true)]
    [InlineData((short)1, false)]
    public void Short_Test(short value, bool isValid)
    {
        var obj = new ShortClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Int_Test(int value, bool isValid)
    {
        var obj = new IntClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(-1L, true)]
    [InlineData(0L, true)]
    [InlineData(1L, false)]
    public void Long_Test(long value, bool isValid)
    {
        var obj = new LongClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(-1.0f, true)]
    [InlineData(0.0f, true)]
    [InlineData(1.0f, false)]
    public void Float_Test(float value, bool isValid)
    {
        var obj = new FloatClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(-1.0, true)]
    [InlineData(0.0, true)]
    [InlineData(1.0, false)]
    public void Double_Test(double value, bool isValid)
    {
        var obj = new DoubleClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(-1.0, true)]
    [InlineData(0.0, true)]
    [InlineData(1.0, false)]
    public void Decimal_Test(decimal value, bool isValid)
    {
        var obj = new DecimalClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    public sealed class BigIntegerClass
    {
        [NonPositive]
        public BigInteger Value { get; set; }
    }

    public sealed class SByteClass
    {
        [NonPositive]
        public sbyte Value { get; set; }
    }

    public sealed class ShortClass
    {
        [NonPositive]
        public short Value { get; set; }
    }

    public sealed class IntClass
    {
        [NonPositive]
        public int Value { get; set; }
    }

    public sealed class LongClass
    {
        [NonPositive]
        public long Value { get; set; }
    }

    public sealed class FloatClass
    {
        [NonPositive]
        public float Value { get; set; }
    }

    public sealed class DoubleClass
    {
        [NonPositive]
        public double Value { get; set; }
    }

    public sealed class DecimalClass
    {
        [NonPositive]
        public decimal Value { get; set; }
    }
}
