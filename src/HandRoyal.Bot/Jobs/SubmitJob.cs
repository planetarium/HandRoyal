using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using static HandRoyal.Bot.EnumerableUtility;

namespace HandRoyal.Bot.Jobs;

public sealed class SubmitJob(ILogger<SubmitJob> logger)
    : JobBase("Submit")
{
    protected override void Verify(IBot bot)
    {
        if (!bot.Properties.TryGetValue<UserData>(out var user))
        {
            throw new InvalidOperationException("User not set");
        }

        if (user.SessionId == default)
        {
            throw new InvalidOperationException("Session not set");
        }
    }

    protected override async Task OnExecuteAsync(IBot bot, CancellationToken cancellationToken)
    {
        var userData = bot.Properties[typeof(UserData)];
        var sessionData = await bot.UpdateUserScopedSessionAsync(cancellationToken);
        while (sessionData.SessionState == GraphQL.SessionState.Active)
        {
            var sessionId = sessionData.SessionId
                ?? throw new InvalidOperationException("Session not set");

            var gloves = SelectMany(sessionData.MyGloves, item => item);
            var gloveIndex = Random.Shared.Next(gloves.Length);
            var gloveType = GloveUtility.GetType(gloves[gloveIndex]);
            await bot.SubmitMoveAsync(sessionId, gloveIndex, cancellationToken);
            UpdateState(bot, gloveType);
            logger.LogInformation(
                "Submitted: {UserId} {SessionId}", bot.Address, sessionId);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            sessionData = await bot.UpdateUserScopedSessionAsync(cancellationToken);
        }

        bot.Properties.Remove(typeof(Options));
    }

    public sealed record class Options
    {
    }
}
