using Microsoft.Extensions.Logging;

namespace HandRoyal.Bot.Jobs;

public sealed class CreateUserJob(ILogger<CreateUserJob> logger)
    : JobBase("CreateUser")
{
    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var userId = bot.Address;
        var name = bot.Name;
        await bot.CreateUserAsync(cancellationToken);
        bot.Properties[typeof(UpdateUserJob.Options)] = new UpdateUserJob.Options
        {
        };
        logger.LogInformation("Created new user: {UserId} {Name}", userId, name);
    }
}
