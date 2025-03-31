using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace HandRoyal.Node.Logging;

internal sealed class DefaultLoggerToSerilog(string categoryName, ILogEventSink serilogSink)
    : ILogger
{
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        var level = ConvertToSerilogLevel(logLevel);
        var textToken = new TextToken(message);
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            level,
            exception,
            new MessageTemplate(message, [textToken]),
            [
                new LogEventProperty("SourceContext", new ScalarValue(categoryName)),
                new LogEventProperty("EventId", new ScalarValue(eventId.Id)),
                new LogEventProperty("EventName", new ScalarValue(eventId.Name ?? string.Empty)),
            ]);

        serilogSink.Emit(logEvent);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    private static LogEventLevel ConvertToSerilogLevel(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => LogEventLevel.Verbose,
        LogLevel.Debug => LogEventLevel.Debug,
        LogLevel.Information => LogEventLevel.Information,
        LogLevel.Warning => LogEventLevel.Warning,
        LogLevel.Error => LogEventLevel.Error,
        LogLevel.Critical => LogEventLevel.Fatal,
        _ => LogEventLevel.Information,
    };
}
