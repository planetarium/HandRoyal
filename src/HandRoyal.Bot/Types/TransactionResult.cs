namespace HandRoyal.Bot.Types;

public sealed record class TransactionResult
{
    public string TxStatus { get; set; } = string.Empty;

    public long? BlockIndex { get; set; }

    public string[] ExceptionNames { get; set; } = [];
}
