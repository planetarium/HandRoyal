using System.ComponentModel.DataAnnotations;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types;

internal sealed record class FavValue
{
    [Required]
    public required decimal Quantity { get; init; }

    [Required]
    public required string Ticker { get; init; }

    [Required]
    public required byte DecimalPlaces { get; init; }

    public Address[]? Minters { get; init; }
}
