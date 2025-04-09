namespace HandRoyal.Bot;

public interface IJobSelector
{
    Task<Type> SelectJobAsync(CancellationToken cancellationToken);
}
