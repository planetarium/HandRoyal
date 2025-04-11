using Microsoft.Extensions.Logging;
using static HandRoyal.Bot.EnumerableUtility;

namespace HandRoyal.Bot.Jobs;

public sealed class SubmitMoveJob(ILogger<SubmitMoveJob> logger)
    : JobBase("SubmitMove")
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
        var sessionData = await bot.UpdateUserScopedSessionAsync(cancellationToken);
        while (sessionData.SessionState == GraphQL.SessionState.Active)
        {
            if (Random.Shared.Next(0, 100) < 40
                && sessionData.CurrentUserMatchState == GraphQL.MatchState.Active)
            {
                await SubmitJobAsync(bot, sessionData, cancellationToken);
            }

            await WaitAsync(500, 2000, cancellationToken);
            sessionData = await bot.UpdateUserScopedSessionAsync(cancellationToken);
        }

        bot.Properties.Remove(typeof(Options));
        bot.Properties[typeof(IdleJob.Options)] = new IdleJob.Options
        {
            Delay = 5000,
        };
    }

    private static async Task WaitAsync(
        int minimumMilliseconds, int maximumMilliseconds, CancellationToken cancellationToken)
    {
        var delay = Random.Shared.Next(minimumMilliseconds, maximumMilliseconds);
        await Task.Delay(delay, cancellationToken);
    }

    private async Task SubmitJobAsync(
        IBot bot, UserScopedSessionData sessionData, CancellationToken cancellationToken)
    {
        var sessionId = sessionData.SessionId
            ?? throw new InvalidOperationException("Session not set");

        var gloves = SelectMany(sessionData.MyGloves, item => item);
        var gloveIndex = Random.Shared.Next(gloves.Length);
        var gloveType = GloveUtility.GetType(gloves[gloveIndex]);
        gloveIndex = Random.Shared.Next(gloves.Length);
        try
        {
            await bot.SubmitMoveAsync(sessionId, gloveIndex, cancellationToken);
            UpdateState(bot, gloveType);
            logger.LogInformation(
                "Submitted: {UserId} {SessionId}", bot.Address, sessionId);
        }
        catch (Exception e)
        {
            UpdateState(bot, "Submit failed");
            logger.LogError(e, "Failed to submit move");
        }
    }

    public sealed record class Options
    {
    }
}
