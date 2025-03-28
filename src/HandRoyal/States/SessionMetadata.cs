using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class SessionMetadata : IEquatable<SessionMetadata>
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

    [Property(3)]
    public int MaximumUser { get; set; } = 8;

    [Property(4)]
    public int MinimumUser { get; set; } = 2;

    [Property(5)]
    public int RemainingUser { get; set; } = 1;

    [Property(6)]
    public long StartAfter { get; set; } = 10;

    [Property(7)]
    public int MaxRounds { get; set; } = 5;

    [Property(8)]
    public long RoundLength { get; set; } = 5;

    [Property(9)]
    public long RoundInterval { get; set; } = 5;

    [Property(10)]
    public int InitialHealthPoint { get; set; } = 100;

    [Property(11)]
    public int NumberOfGloves { get; set; } = 5;

    [Property(12)]
    public ImmutableArray<Address> Users { get; set; } = [];

    public override int GetHashCode() => ModelUtility.GetHashCode(this);

    public bool Equals(SessionMetadata? other) => ModelUtility.Equals(this, other);
}
