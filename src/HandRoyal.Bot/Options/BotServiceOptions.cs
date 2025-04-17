using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Libplanet.Node.Options;

namespace HandRoyal.Bot.Options;

[Options(Position)]
public sealed class BotServiceOptions : OptionsBase<BotServiceOptions>
{
    public const string Position = "Bot";
    public const int DefaultBotCount = 10;

    public bool IsEnabled { get; set; }

    [Range(0, 100)]
    [Description("The maximum number of bots allowed in the service (0-100).")]
    public int BotCount { get; set; } = DefaultBotCount;

    [Range(0, 100)]
    [Description("The number of organiser bots to create (0-100).")]
    public int OrganiserBotCount { get; set; } = 0;

    [Required]
    [Description("The GraphQL endpoint URL for the bot service.")]
    public string GraphqlEndpoint { get; set; } = string.Empty;
}
