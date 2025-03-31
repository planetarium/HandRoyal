using Serilog.Core;

namespace HandRoyal.Node.Logging;

internal sealed class DefaultLoggerToSerilogSink(ILogEventSink serilogSink) : ILoggerProvider
{
    private readonly ILogEventSink _serilogSink = serilogSink;

    public ILogger CreateLogger(string categoryName)
        => new DefaultLoggerToSerilog(categoryName, _serilogSink);

    void IDisposable.Dispose()
    {
        // Oo nothing
    }
}
