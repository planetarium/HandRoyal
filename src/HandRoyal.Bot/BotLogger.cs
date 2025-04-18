using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot;

internal sealed class BotLogger(ILogger logger) : ILogger
{
    private readonly StringBuilder _logBulider = new();
    private readonly Channel<string> _logChannel = Channel.CreateUnbounded<string>();

    private readonly object _lock = new();

    public async IAsyncEnumerable<string> GetLogStream(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string initialLog;
        lock (_lock)
        {
            initialLog = _logBulider.ToString();
        }

        yield return initialLog;

        while (await _logChannel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_logChannel.Reader.TryRead(out var log))
            {
                yield return log;
            }
        }
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => logger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var logLevelString = GetLogLevelString(logLevel);
        var message = $"[{dateTime} {logLevelString}] {formatter(state, exception)}";
        logger.Log(logLevel, eventId, state, exception, formatter);
        _ = Task.Run(() => AppendLog(message));
    }

    private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => "TRACE",
        LogLevel.Debug => "DEBUG",
        LogLevel.Information => "INFO",
        LogLevel.Warning => "WARN",
        LogLevel.Error => "ERROR",
        LogLevel.Critical => "CRIT",
        _ => "UNKNOWN",
    };

    private void AppendLog(string message)
    {
        lock (_lock)
        {
            if (_logBulider.Length > 0)
            {
                _logBulider.AppendLine();
            }

            _logBulider.Append(message);
        }

        _logChannel.Writer.TryWrite(message);
    }
}
