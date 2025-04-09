namespace HandRoyal.Bot;

public interface IJob
{
    string Name { get; }

    Task ExecuteAsync(IBot bot, CancellationToken cancellationToken);
}
