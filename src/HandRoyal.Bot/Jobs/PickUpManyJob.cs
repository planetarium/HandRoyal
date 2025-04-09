using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class PickUpManyJob(ILogger<PickUpManyJob> logger)
    : JobBase("PickUpMany")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        await bot.PickUpManyAsync(cancellationToken);
        bot.Properties[typeof(UpdateUserJob.Options)] = new UpdateUserJob.Options();
        logger.LogInformation("Picked up many: {BotId}", bot.Address);
    }
}
