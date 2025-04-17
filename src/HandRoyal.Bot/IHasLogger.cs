using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot;

public interface IHasLogger
{
    ILogger Logger { get; }
}
