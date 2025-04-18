namespace HandRoyal.Bot;

public interface IJobObserver
{
    void OnStarted(Type type, string name);

    void OnFinished(Type type, string name, Exception? exception);

    void OnUpdated(Type type, string name, string state);
}
