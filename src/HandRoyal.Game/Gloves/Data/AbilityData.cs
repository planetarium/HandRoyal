namespace HandRoyal.Game.Gloves.Data;

public class AbilityData
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int Duration { get; set; }

    public required int[] Parameters { get; set; }
}
