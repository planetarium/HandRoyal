using Libplanet.Crypto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot;

internal sealed class JobContext(IBot bot) : IJobContext
{
    private readonly ILogger _logger = GetLogger(bot);

    public string Name => bot.Name;

    public Address Address => bot.Address;

    public BotPropertyCollection Properties => bot.Properties;

    IDisposable? ILogger.BeginScope<TState>(TState state)
        => _logger.BeginScope(state);

    public object? GetService(Type serviceType) => bot.GetService(serviceType);

    bool ILogger.IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

    void ILogger.Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
        => _logger.Log<TState>(logLevel, eventId, state, exception, formatter);

    byte[] IJobContext.Sign(byte[] message) => bot.Sign(message);

    public void OnJobUpdated(JobBase job, string state)
    {
        if (bot is IJobObserver jobObserver)
        {
            jobObserver.OnUpdated(job.GetType(), job.Name, state);
        }
    }

    private static ILogger GetLogger(IBot bot)
    {
        if (bot is IHasLogger hasLogger)
        {
            return hasLogger.Logger;
        }

        return bot.GetRequiredService<ILoggerFactory>().CreateLogger(bot.GetType());
    }
}
