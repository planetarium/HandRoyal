namespace HandRoyal.Bot;

public class JobEventArgs : EventArgs
{
    public JobInfo JobInfo { get; init; } = JobInfo.Empty;
}
