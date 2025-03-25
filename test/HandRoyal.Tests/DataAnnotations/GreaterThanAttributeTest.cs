using System.ComponentModel.DataAnnotations;
using System.Numerics;
using HandRoyal.DataAnnotations;

namespace HandRoyal.Tests.DataAnnotations;

public partial class GreaterThanAttributeTest
{
    [Theory]
    [InlineData((byte)15, true)]
    [InlineData((byte)5, false)]
    public void ByteClass_Value_Test(byte value, bool expectedResult)
    {
        var obj = new ByteClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData((sbyte)15, true)]
    [InlineData((sbyte)5, false)]
    public void SByteClass_Value_Test(sbyte value, bool expectedResult)
    {
        var obj = new SByteClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData((short)15, true)]
    [InlineData((short)5, false)]
    public void ShortClass_Value_Test(short value, bool expectedResult)
    {
        var obj = new ShortClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData((ushort)15, true)]
    [InlineData((ushort)5, false)]
    public void UShortClass_Value_Test(ushort value, bool expectedResult)
    {
        var obj = new UShortClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15, true)]
    [InlineData(5, false)]
    public void IntClass_Value_Test(int value, bool expectedResult)
    {
        var obj = new IntClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15U, true)]
    [InlineData(5U, false)]
    public void UIntClass_Value_Test(uint value, bool expectedResult)
    {
        var obj = new UIntClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15L, true)]
    [InlineData(5L, false)]
    public void LongClass_Value_Test(long value, bool expectedResult)
    {
        var obj = new LongClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15UL, true)]
    [InlineData(5UL, false)]
    public void ULongClass_Value_Test(ulong value, bool expectedResult)
    {
        var obj = new ULongClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15.0F, true)]
    [InlineData(5.0F, false)]
    public void FloatClass_Value_Test(float value, bool expectedResult)
    {
        var obj = new FloatClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15.0, true)]
    [InlineData(5.0, false)]
    public void DoubleClass_Value_Test(double value, bool expectedResult)
    {
        var obj = new DoubleClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(15.0, true)]
    [InlineData(5.0, false)]
    public void DecimalClass_Value_Test(decimal value, bool expectedResult)
    {
        var obj = new DecimalClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("15", true)]
    [InlineData("5", false)]
    public void BigIntegerClass_Value_Test(string value, bool expectedResult)
    {
        var bigInteger = BigInteger.Parse(value);
        var obj = new BigIntegerClass { Value = bigInteger };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    private class ByteClass
    {
        [GreaterThan((byte)10)]
        public byte Value { get; set; }
    }

    private class SByteClass
    {
        [GreaterThan((sbyte)10)]
        public sbyte Value { get; set; }
    }

    private class ShortClass
    {
        [GreaterThan((short)10)]
        public short Value { get; set; }
    }

    private class UShortClass
    {
        [GreaterThan((ushort)10)]
        public ushort Value { get; set; }
    }

    private class IntClass
    {
        [GreaterThan(10)]
        public int Value { get; set; }
    }

    private class UIntClass
    {
        [GreaterThan(10U)]
        public uint Value { get; set; }
    }

    private class LongClass
    {
        [GreaterThan(10L)]
        public long Value { get; set; }
    }

    private class ULongClass
    {
        [GreaterThan(10UL)]
        public ulong Value { get; set; }
    }

    private class FloatClass
    {
        [GreaterThan(10.0F)]
        public float Value { get; set; }
    }

    private class DoubleClass
    {
        [GreaterThan(10.0)]
        public double Value { get; set; }
    }

    private class DecimalClass
    {
        [GreaterThan(10.0)]
        public decimal Value { get; set; }
    }

    private class BigIntegerClass
    {
        [GreaterThan("10")]
        public BigInteger Value { get; set; }
    }
}
