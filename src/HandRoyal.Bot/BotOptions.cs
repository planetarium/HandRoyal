using System.ComponentModel.DataAnnotations;
using Libplanet.Crypto;

namespace HandRoyal.Bot;

public sealed record class BotOptions
{
    public required PrivateKey PrivateKey { get; init; }

    [Required]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public required string Name { get; init; }

    [Required]
    public required Uri GraphqlUrl { get; init; }

    public IServiceProvider? ServiceProvider { get; init; }

    public BotOptions EnsureComplete()
    {
        return this;
    }
}
