namespace HandRoyal.Bot;

public interface IBotService
{
    bool IsEnabled { get; }

    BotCollection Bots { get; }

    void RegisterBot(IBot bot);
}
