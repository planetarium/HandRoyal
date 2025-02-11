using System.ComponentModel.DataAnnotations;

namespace HandRoyal.Node.Explorer.Types;

public sealed record class TipEventData
{
    [Required]
    public required long Height { get; init; }
}
