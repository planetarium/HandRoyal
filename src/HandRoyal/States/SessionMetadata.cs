using System.ComponentModel.DataAnnotations;
using HandRoyal.Serialization;
using Libplanet.Crypto;

namespace HandRoyal.States;

[Model(Version = 1)]
public sealed record class SessionMetadata
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
    public long RoundInterval { get; set; } = 5;

    [Property(7)]
    public long WaitingInterval { get; set; } = 10;
}
