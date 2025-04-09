using HandRoyal.States;
using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class UpdateUserJob(ILogger<UpdateUserJob> logger)
    : JobBase("UpdateUser")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        if (!bot.Properties.TryGetValue<Options>(out _))
        {
            return;
        }

        bot.Properties[typeof(User)] = await bot.GetUserDataAsync(cancellationToken);
        bot.Properties.Remove(typeof(Options));
        logger.LogInformation("User data updated");
    }

    public sealed record class Options
    {
    }
}
