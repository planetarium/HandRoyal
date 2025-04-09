namespace HandRoyal.Bot;

public interface IJobObserver
{
    void OnJobStarted(Type type, string name);

    void OnJobFinished(Type type, string name, Exception? exception);

    void OnJobUpdated(Type type, string name, string state);
}
