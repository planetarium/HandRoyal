using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Libplanet.Crypto;

namespace HandRoyal.Node.Explorer.Types;

public sealed record class FavValue
{
    [Required]
    public required string Quantity { get; init; }

    [Required]
    public required string Ticker { get; init; }

    [Required]
    public required byte DecimalPlaces { get; init; }

    public Address[]? Minters { get; init; }
}
