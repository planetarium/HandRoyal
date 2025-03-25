using System.ComponentModel.DataAnnotations;
using HandRoyal.DataAnnotations;

namespace HandRoyal.Tests.DataAnnotations;

public partial class LessThanAttributeTest
{
    [Theory]
    [InlineData(5, 10, true)]
    [InlineData(15, 10, false)]
    public void StaticType_Value_Test(int value, int compareValue, bool expectedResult)
    {
        StaticValues.CompareValue = compareValue;
        var obj = new StaticTypeClass { Value = value };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(5, 10, true)]
    [InlineData(15, 10, false)]
    public void SelfType_Value_Test(int value, int compareValue, bool expectedResult)
    {
        var obj = new SelfTypeClass { Value = value, CompareValue = compareValue };
        var context = new ValidationContext(obj);
        var result = Validator.TryValidateObject(
            obj, context, validationResults: null, validateAllProperties: true);

        Assert.Equal(expectedResult, result);
    }

    private static class StaticValues
    {
        public static int CompareValue { get; set; } = 10;
    }

    private class StaticTypeClass
    {
        [LessThan(typeof(StaticValues), nameof(StaticValues.CompareValue))]
        public int Value { get; set; }
    }

    private class SelfTypeClass
    {
        [LessThan(targetType: null, nameof(CompareValue))]
        public int Value { get; set; }

        public int CompareValue { get; set; }
    }
}
