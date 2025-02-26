using System.ComponentModel.DataAnnotations;
using HandRoyal.DataAnnotations;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class SessionMetadata : IValidatableObject
{
    public static SessionMetadata Default { get; } = new SessionMetadata
    {
        Id = default,
        Organizer = default,
        Prize = default,
    };

    [Required]
    [Property(0)]
    public required Address Id { get; init; }

    [Required]
    [Property(1)]
    public required Address Organizer { get; init; }

    [Required]
    [Property(2)]
    public required Address Prize { get; init; }

    [Positive]
    [Property(3)]
    public int MaximumUser { get; set; } = 8;

    [Positive]
    [Property(4)]
    public int MinimumUser { get; set; } = 2;

    [Positive]
    [Property(5)]
    public int RemainingUser { get; set; } = 1;

    [Positive]
    [Property(6)]
    public long StartAfter { get; set; } = 10;

    [Positive]
    [Property(7)]
    public long RoundLength { get; set; } = 5;

    [Property(8)]
    public long RoundInterval { get; set; } = 5;

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        if (MinimumUser > MaximumUser)
        {
            yield return new ValidationResult(
                "MinimumUser cannot be greater than MaximumUser",
                [nameof(MinimumUser), nameof(MaximumUser)]);
        }

        if (RemainingUser > MaximumUser)
        {
            yield return new ValidationResult(
                "RemainingUser cannot be greater than MaximumUser",
                [nameof(RemainingUser), nameof(MaximumUser)]);
        }
    }
}
