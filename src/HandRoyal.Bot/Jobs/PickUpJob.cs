using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class PickUpJob(ILogger<PickUpJob> logger)
    : JobBase("PickUp")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        await bot.PickUpAsync(cancellationToken);
        bot.Properties[typeof(UpdateUserJob.Options)] = new UpdateUserJob.Options();
        logger.LogInformation("Picked up: {BotId}", bot.Address);
    }
}
