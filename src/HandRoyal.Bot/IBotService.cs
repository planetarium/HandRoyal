namespace HandRoyal.Bot;

public interface IBotService
{
    bool IsEnabled { get; }

    BotCollection Bots { get; }

    IBot AddNew();
}
