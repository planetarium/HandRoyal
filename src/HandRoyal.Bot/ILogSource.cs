namespace HandRoyal.Pages;

public interface ILogSource
{
    IAsyncEnumerable<string> GetLogStream(CancellationToken cancellationToken);
}
