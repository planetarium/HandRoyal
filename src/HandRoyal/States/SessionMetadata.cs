using System.ComponentModel.DataAnnotations;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;
using static HandRoyal.BencodexUtility;

namespace HandRoyal.States;

public sealed record class SessionMetadata : IBencodable
{
    public SessionMetadata()
    {
    }

    public SessionMetadata(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.", nameof(value));
        }

        Id = ToAddress(list, 0);
        Organizer = ToAddress(list, 1);
        Prize = ToAddress(list, 2);
        MaximumUser = ToInt32(list, 3);
        MinimumUser = ToInt32(list, 4);
        RemainingUser = ToInt32(list, 5);
        RoundInterval = ToInt64(list, 6);
        WaitingInterval = ToInt64(list, 7);
    }

    public IValue Bencoded => new List(
        ToValue(Id),
        ToValue(Organizer),
        ToValue(Prize),
        ToValue(MaximumUser),
        ToValue(MinimumUser),
        ToValue(RemainingUser),
        ToValue(RoundInterval),
        ToValue(WaitingInterval));

    public static SessionMetadata Default { get; } = new SessionMetadata
    {
        Id = default,
        Organizer = default,
        Prize = default,
    };

    [Required]
    public required Address Id { get; init; }

    [Required]
    public required Address Organizer { get; init; }

    [Required]
    public required Address Prize { get; init; }

    public int MaximumUser { get; set; } = 8;

    public int MinimumUser { get; set; } = 2;

    public int RemainingUser { get; set; } = 1;

    public long RoundInterval { get; set; } = 5;

    public long WaitingInterval { get; set; } = 10;
}
