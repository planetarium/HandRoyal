using HandRoyal.Bot.GraphQL;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using Microsoft.Extensions.DependencyInjection;
using static HandRoyal.Bot.EnumerableUtility;

namespace HandRoyal.Bot;

public static class IJobContextExtensions
{
    public static async Task CreateUserAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.CreateUser.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.CreateUser);

        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task CreateSessionAsync(
        this IJobContext @this,
        Address sessionId,
        CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.CreateSession.ExecuteAsync(
            sessionId: sessionId,
            prize: new Address("0x0000000000000000000000000000000000000000"),
            maximumUser: 8,
            minimumUser: 2,
            remainingUser: 1,
            startAfter: Random.Shared.Next(10, 20),
            maxRounds: 5,
            roundLength: 10,
            roundInterval: 10,
            initialHealthPoint: 100,
            users: [],
            cancellationToken: cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.CreateSession);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task JoinSessionAsync(
        this IJobContext @this, Address sessionId, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var user = (UserData)@this.Properties[typeof(UserData)];
        var ownedGloves = SelectMany(user.OwnedGloves, item => (item.Id, item.Count));
        var gloves = GloveUtility.GetRandomGloves(ownedGloves, 5);
        var plainValue = await client.JoinSession.ExecuteAsync(
            sessionId: sessionId,
            gloves: gloves,
            cancellationToken: cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.JoinSession);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task PickUpAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.PickUp.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.PickUp);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task PickUpManyAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.PickUpMany.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.PickUpMany);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task RegisterMatchingAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var user = (UserData)@this.Properties[typeof(UserData)];
        var ownedGloves = SelectMany(user.OwnedGloves, item => (item.Id, item.Count));
        var gloves = GloveUtility.GetRandomGloves(ownedGloves, 8);
        var plainValue = await client.RegisterMatching.ExecuteAsync(gloves, cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.RegisterMatching);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task CancelMatchingAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.CancelMatching.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.CancelMatching);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task SubmitMoveAsync(
        this IJobContext @this,
        Address sessionId,
        int gloveIndex,
        CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.SubmitMove.ExecuteAsync(
            sessionId, gloveIndex, cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.SubmitMove);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static IAsyncEnumerable<IOnTipChanged_OnTipChanged> OnTiChangedAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var result = client.OnTipChanged.Watch();
        return result.AsAsyncEnumerableAsync(item => item.OnTipChanged, cancellationToken);
    }

    public static IAsyncEnumerable<IOnSessionChanged_OnSessionChanged> OnSessionChangedAsync(
        this IJobContext @this, Address sessionId, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var result = client.OnSessionChanged.Watch(sessionId, @this.Address);
        return result.AsAsyncEnumerableAsync(item => item.OnSessionChanged, cancellationToken);
    }

    public static async Task<Address[]> GetJoinableSessionsAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var sessions = await client.GetJoinableSessions.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.StateQuery?.Sessions);
        var query = from session in sessions
                    where session is not null
                    where session.State == GraphQL.SessionState.Active
                    let metadata = session.Metadata
                    where metadata is not null
                    select metadata.Id;
        return [.. query];
    }

    public static async Task<SessionData> GetSessionAsync(
        this IJobContext @this, Address sessionId, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var session = await client.GetSession.ExecuteAsync(sessionId, cancellationToken)
            .ResolveAsync(item => item.StateQuery?.Session);
        return session;
    }

    public static async Task<UserScopedSessionData> UpdateUserScopedSessionAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var userData = (UserData)@this.Properties[typeof(UserData)];
        var session = await GetUserScopedSessionAsync(@this, userData.SessionId, cancellationToken);
        @this.Properties[typeof(UserScopedSessionData)] = session;
        return session;
    }

    public static async Task<UserScopedSessionData> GetUserScopedSessionAsync(
        this IJobContext @this, Address sessionId, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var userId = @this.Address;
        var session = await client.GetUserScopedSession.ExecuteAsync(
            sessionId, userId, cancellationToken)
            .ResolveAsync(item => item.StateQuery?.UserScopedSession);
        return session;
    }

    public static async Task<UserData> UpdateUserDataAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var user = await GetUserDataAsync(@this, cancellationToken);
        @this.Properties[typeof(UserData)] = user;
        return user;
    }

    public static async Task<UserData> GetUserDataAsync(
        this IJobContext @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var userId = @this.Address;
        var userData = await client.GetUserData.ExecuteAsync(userId, cancellationToken)
            .ResolveAsync(item => item.StateQuery?.GetUserData);
        return userData;
    }

    private static string Sign(IJobContext context, string hex)
    {
        var bytes = ByteUtil.ParseHex(hex);
        var signedBytes = context.Sign(bytes);
        return ByteUtil.Hex(signedBytes);
    }

    private static async Task<TxId> ExecuteTransactionAsync(
        IJobContext context,
        string plainValue,
        CancellationToken cancellationToken)
    {
        var address = context.Address;
        var client = context.GetRequiredService<IGraphQLClient>();
        var unsignedTx = await client.UnsignedTransaction.ExecuteAsync(
            address, plainValue, cancellationToken)
            .ResolveAsync(item => item.Transaction?.UnsignedTransaction);
        var signature = Sign(context, unsignedTx);
        return await client.StageTransaction.ExecuteAsync(
            unsignedTx, signature, cancellationToken)
            .ResolveAsync(item => item.StageTransaction);
    }

    private static async Task PollTransactionAsync(
        IJobContext context,
        TxId txId,
        CancellationToken cancellationToken = default)
    {
        var client = context.GetRequiredService<IGraphQLClient>();
        var interval = TimeSpan.FromSeconds(1);
        var timeout = TimeSpan.FromMinutes(5);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                var result = await client.TransactionResult.ExecuteAsync(txId, cts.Token)
                    .ResolveAsync(item => item.Transaction?.TransactionResult);

                if (result != null)
                {
                    if (result.TxStatus == TxStatus.Staging || result.TxStatus == TxStatus.Included)
                    {
                        await Task.Delay(interval, cts.Token);
                        continue;
                    }

                    if (result.TxStatus == TxStatus.Success)
                    {
                        return;
                    }

                    throw new InvalidOperationException(
                        $"Transaction failed with status: {result.TxStatus}");
                }

                await Task.Delay(interval, cts.Token);
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                break;
            }
        }

        var message = $"Transaction polling timed out after {timeout.TotalSeconds} " +
                      $"seconds: {txId}";
        throw new TimeoutException(message);
    }
}
