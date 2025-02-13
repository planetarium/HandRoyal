using System.ComponentModel.DataAnnotations;
using Libplanet.Types.Blocks;

namespace HandRoyal.Node.Explorer.Types;

public sealed record class TipEventData
{
    [Required]
    public required long Height { get; init; }

    [Required]
    public required BlockHash Hash { get; init; }
}
