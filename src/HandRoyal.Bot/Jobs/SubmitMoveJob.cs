using Microsoft.Extensions.Logging;
using static HandRoyal.Bot.EnumerableUtility;

namespace HandRoyal.Bot.Jobs;

public sealed class SubmitMoveJob : JobBase
{
    protected override void Verify(IJobContext context)
    {
        if (!context.Properties.TryGetValue<UserData>(out var user))
        {
            throw new InvalidOperationException("User not set");
        }

        if (user.SessionId == default)
        {
            throw new InvalidOperationException("Session not set");
        }
    }

    protected override async Task OnExecuteAsync(
        IJobContext context, CancellationToken cancellationToken)
    {
        var sessionData = await context.UpdateUserScopedSessionAsync(cancellationToken);
        while (sessionData.SessionState == GraphQL.SessionState.Active)
        {
            if (Random.Shared.Next(0, 100) < 40
                && sessionData.CurrentUserMatchState == GraphQL.MatchState.Active)
            {
                await SubmitJobAsync(context, sessionData, cancellationToken);
            }

            await WaitAsync(500, 2000, cancellationToken);
            sessionData = await context.UpdateUserScopedSessionAsync(cancellationToken);
        }

        context.Properties.Remove(typeof(Options));
        context.Properties[typeof(IdleJob.Options)] = new IdleJob.Options
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
        IJobContext context, UserScopedSessionData sessionData, CancellationToken cancellationToken)
    {
        var sessionId = sessionData.SessionId
            ?? throw new InvalidOperationException("Session not set");

        var gloves = SelectMany(sessionData.MyGloves, item => item);
        var gloveIndex = Random.Shared.Next(gloves.Length);
        var gloveType = GloveUtility.GetType(gloves[gloveIndex]);
        gloveIndex = Random.Shared.Next(gloves.Length);
        try
        {
            await context.SubmitMoveAsync(sessionId, gloveIndex, cancellationToken);
            UpdateState(context, gloveType);
            context.LogInformation(
                "Submitted: {UserId} {SessionId}", context.Address, sessionId);
        }
        catch (Exception e)
        {
            UpdateState(context, "Submit failed");
            context.LogError(e, "Failed to submit move");
        }
    }

    public sealed record class Options
    {
    }
}
