namespace HandRoyal.Bot.Types;

public sealed record class TipChangedEventData
{
    public long Height { get; set; }

    public string Hash { get; set; } = string.Empty;
}
