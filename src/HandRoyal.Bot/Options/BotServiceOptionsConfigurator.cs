using Libplanet.Node.Options;

namespace HandRoyal.Bot.Options;

internal sealed class BotServiceOptionsConfigurator : OptionsConfiguratorBase<BotServiceOptions>
{
    protected override void OnConfigure(BotServiceOptions options)
    {
        // Data Annotation 특성으로 유효성 검사가 처리됩니다.
    }
}
