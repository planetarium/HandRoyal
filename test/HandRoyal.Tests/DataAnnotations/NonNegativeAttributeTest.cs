using System.ComponentModel.DataAnnotations;
using System.Numerics;
using HandRoyal.DataAnnotations;

namespace HandRoyal.Tests.DataAnnotations;

public sealed class NonNegativeAttributeTest
{
    [Theory]
    [InlineData("1234567890123456789012345678901234567890", true)]
    [InlineData("0", true)]
    [InlineData("-1234567890123456789012345678901234567890", false)]
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
    [InlineData((sbyte)1, true)]
    [InlineData((sbyte)0, true)]
    [InlineData((sbyte)-1, false)]
    public void SByte_Test(sbyte value, bool isValid)
    {
        var obj = new SByteClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData((byte)1, true)]
    [InlineData((byte)0, true)]
    public void Byte_Test(byte value, bool isValid)
    {
        var obj = new ByteClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData((short)1, true)]
    [InlineData((short)0, true)]
    [InlineData((short)-1, false)]
    public void Short_Test(short value, bool isValid)
    {
        var obj = new ShortClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData((ushort)1, true)]
    [InlineData((ushort)0, true)]
    public void UShort_Test(ushort value, bool isValid)
    {
        var obj = new UShortClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, true)]
    [InlineData(-1, false)]
    public void Int_Test(int value, bool isValid)
    {
        var obj = new IntClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, true)]
    public void UInt_Test(uint value, bool isValid)
    {
        var obj = new UIntClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, true)]
    [InlineData(-1, false)]
    public void Long_Test(long value, bool isValid)
    {
        var obj = new LongClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, true)]
    public void ULong_Test(ulong value, bool isValid)
    {
        var obj = new ULongClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1.0f, true)]
    [InlineData(0.0f, true)]
    [InlineData(-1.0f, false)]
    public void Float_Test(float value, bool isValid)
    {
        var obj = new FloatClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1.0, true)]
    [InlineData(0.0, true)]
    [InlineData(-1.0, false)]
    public void Double_Test(double value, bool isValid)
    {
        var obj = new DoubleClass { Value = value };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(isValid, result);
    }

    [Theory]
    [InlineData(1.0, true)]
    [InlineData(0.0, true)]
    [InlineData(-1.0, false)]
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
        [NonNegative]
        public BigInteger Value { get; set; }
    }

    public sealed class SByteClass
    {
        [NonNegative]
        public sbyte Value { get; set; }
    }

    public sealed class ByteClass
    {
        [NonNegative]
        public byte Value { get; set; }
    }

    public sealed class ShortClass
    {
        [NonNegative]
        public short Value { get; set; }
    }

    public sealed class UShortClass
    {
        [NonNegative]
        public ushort Value { get; set; }
    }

    public sealed class IntClass
    {
        [NonNegative]
        public int Value { get; set; }
    }

    public sealed class UIntClass
    {
        [NonNegative]
        public uint Value { get; set; }
    }

    public sealed class LongClass
    {
        [NonNegative]
        public long Value { get; set; }
    }

    public sealed class ULongClass
    {
        [NonNegative]
        public ulong Value { get; set; }
    }

    public sealed class FloatClass
    {
        [NonNegative]
        public float Value { get; set; }
    }

    public sealed class DoubleClass
    {
        [NonNegative]
        public double Value { get; set; }
    }

    public sealed class DecimalClass
    {
        [NonNegative]
        public decimal Value { get; set; }
    }
}
