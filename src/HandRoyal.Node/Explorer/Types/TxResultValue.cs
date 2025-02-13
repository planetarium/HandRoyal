using System.ComponentModel.DataAnnotations;

namespace HandRoyal.Node.Explorer.Types;

public sealed record class TxResultValue
{
    [Required]
    public required TxStatus TxStatus { get; init; }

    public long? BlockIndex { get; init; }

    public string?[]? ExceptionNames { get; init; }
}
