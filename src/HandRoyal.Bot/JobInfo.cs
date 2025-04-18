namespace HandRoyal.Bot;

public readonly record struct JobInfo
{
    public static JobInfo Empty { get; } = new JobInfo
    {
        Type = typeof(void),
        Name = string.Empty,
        State = string.Empty,
        StartTime = DateTimeOffset.MinValue,
        FinishTime = DateTimeOffset.MinValue,
    };

    public required Type Type { get; init; }

    public required string Name { get; init; }

    public required string State { get; init; }

    public readonly DateTimeOffset StartTime { get; init; }

    public readonly DateTimeOffset FinishTime { get; init; }

    public readonly Exception? Exception { get; init; }

    public readonly bool IsStarted => StartTime != DateTimeOffset.MinValue;

    public readonly bool IsCompleted => FinishTime != DateTimeOffset.MinValue;

    public readonly bool IsCompletedSuccessfully => IsCompleted && Exception is null;

    public readonly bool IsFaulted => IsCompleted && Exception is not null;
}
