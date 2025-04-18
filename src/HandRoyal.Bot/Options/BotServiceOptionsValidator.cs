using Libplanet.Node.Options;

namespace HandRoyal.Bot.Options;

internal sealed class BotServiceOptionsValidator : OptionsValidatorBase<BotServiceOptions>
{
    protected override void OnValidate(string? name, BotServiceOptions options)
    {
        // Data Annotation 특성으로 유효성 검사가 처리됩니다.
    }
}
