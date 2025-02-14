using System.ComponentModel.DataAnnotations;

namespace HandRoyal.Explorer.Types;

internal sealed record class TxResultValue
{
    [Required]
    public required TxStatus TxStatus { get; init; }

    public long? BlockIndex { get; init; }

    public string?[]? ExceptionNames { get; init; }
}
