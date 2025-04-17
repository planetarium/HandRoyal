using Libplanet.Crypto;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot;

public interface IJobContext : IServiceProvider, ILogger
{
    public string Name { get; }

    public Address Address { get; }

    public BotPropertyCollection Properties { get; }

    public byte[] Sign(byte[] message);
}
