using System.ComponentModel.DataAnnotations;
using System.Numerics;
using HandRoyal.DataAnnotations;

namespace HandRoyal.Tests.DataAnnotations;

public sealed class ValidateObjectAttributeTest
{
    [Fact]
    public void Validate_Nothing_Test()
    {
        var obj = new ParentClass
        {
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, false);
        Assert.True(result);
    }

    [Fact]
    public void Validate_ValidateAllProperties_FailTest()
    {
        var obj = new ParentClass
        {
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var result = Validator.TryValidateObject(obj, context, results, true);
        Assert.False(result);
    }

    public sealed class ParentClass
    {
        [ValidateObject]
        public ChildClass Child1 { get; set; } = new();

        [ValidateObject(ValidateAllProperties = true)]
        public ChildClass Child2 { get; set; } = new();
    }

    public sealed class ChildClass : IValidatableObject
    {
        [Positive]
        public BigInteger Value { get; set; }

        public string Text { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!int.TryParse(Text, out var _))
            {
                yield return new ValidationResult("Text must be a number", [nameof(Text)]);
            }
        }
    }
}
