using System.Collections.Immutable;
using HandRoyal.Bot.GraphQL;
using HandRoyal.States;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using Microsoft.Extensions.DependencyInjection;

namespace HandRoyal.Bot;

public static class IBotExtensions
{
    public static async Task CreateUserAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.CreateUser.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.CreateUser);

        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task CreateSessionAsync(
        this IBot @this,
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
        this IBot @this, Address sessionId, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var user = (User)@this.Properties[typeof(User)];
        var gloves = GloveUtility.GetRandomGloves(user.OwnedGloves, 5);
        var plainValue = await client.JoinSession.ExecuteAsync(
            sessionId: sessionId,
            gloves: gloves,
            cancellationToken: cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.JoinSession);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task PickUpAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.PickUp.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.PickUp);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task PickUpManyAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.PickUpMany.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.PickUpMany);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task RegisterMatchingAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var user = (User)@this.Properties[typeof(User)];
        var gloves = GloveUtility.GetRandomGloves(user.OwnedGloves, 5);
        var plainValue = await client.RegisterMatching.ExecuteAsync(gloves, cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.RegisterMatching);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static async Task CancelMatchingAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var plainValue = await client.CancelMatching.ExecuteAsync(cancellationToken)
            .ResolveAsync(item => item.ActionQuery?.CancelMatching);
        var txId = await ExecuteTransactionAsync(@this, plainValue, cancellationToken);
        await PollTransactionAsync(@this, txId, cancellationToken);
    }

    public static IAsyncEnumerable<IOnTipChanged_OnTipChanged> OnTiChangedAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var result = client.OnTipChanged.Watch();
        return result.AsAsyncEnumerableAsync(item => item.OnTipChanged, cancellationToken);
    }

    public static IAsyncEnumerable<IOnSessionChanged_OnSessionChanged> OnSessionChangedAsync(
        this IBot @this, Address sessionId, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var result = client.OnSessionChanged.Watch(sessionId, @this.Address);
        return result.AsAsyncEnumerableAsync(item => item.OnSessionChanged, cancellationToken);
    }

    public static Task<Address[]> GetSessionsAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        // var client = new GraphQLClient(@this.EndPoint);
        // return client.GetSessionsAsync(cancellationToken);
        throw new NotImplementedException();
    }

    public static async Task<User> UpdateUserDataAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var user = await GetUserDataAsync(@this, cancellationToken);
        @this.Properties[typeof(User)] = user;
        return user;
    }

    public static async Task<User> GetUserDataAsync(
        this IBot @this, CancellationToken cancellationToken)
    {
        var client = @this.GetRequiredService<IGraphQLClient>();
        var userId = @this.Address;
        var userData = await client.GetUserData.ExecuteAsync(userId, cancellationToken)
            .ResolveAsync(item => item.StateQuery?.GetUserData);
        return new User
        {
            Id = userData.Id,
            Name = userData.Name ?? string.Empty,
            RegisteredGloves = SelectMany(userData.RegisteredGloves),
            OwnedGloves = SelectMany(userData.OwnedGloves, item =>
            {
                return new GloveInfo
                {
                    Id = item.Id,
                    Count = item.Count,
                };
            }),
            SessionId = userData.SessionId,
            EquippedGlove = userData.EquippedGlove,
        };
    }

    private static ImmutableArray<T> SelectMany<T>(IReadOnlyList<T>? source)
    {
        if (source is null)
        {
            return [];
        }

        var builder = ImmutableArray.CreateBuilder<T>(source.Count);
        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            builder.Add(item);
        }

        return builder.ToImmutable();
    }

    private static ImmutableArray<T> SelectMany<TSource, T>(
        IReadOnlyList<TSource?>? source, Func<TSource, T> selector)
    {
        if (source is null)
        {
            return [];
        }

        var builder = ImmutableArray.CreateBuilder<T>(source.Count);
        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            builder.Add(selector(item));
        }

        return builder.ToImmutable();
    }

    private static string Sign(IBot bot, string hex)
    {
        var bytes = ByteUtil.ParseHex(hex);
        var signedBytes = bot.Sign(bytes);
        return ByteUtil.Hex(signedBytes);
    }

    private static async Task<TxId> ExecuteTransactionAsync(
        IBot bot,
        string plainValue,
        CancellationToken cancellationToken)
    {
        var address = bot.Address;
        var client = bot.GetRequiredService<IGraphQLClient>();
        var unsignedTx = await client.UnsignedTransaction.ExecuteAsync(
            address, plainValue, cancellationToken)
            .ResolveAsync(item => item.Transaction?.UnsignedTransaction);
        var signature = Sign(bot, unsignedTx);
        return await client.StageTransaction.ExecuteAsync(
            unsignedTx, signature, cancellationToken)
            .ResolveAsync(item => item.StageTransaction);
    }

    private static async Task PollTransactionAsync(
        IBot bot,
        TxId txId,
        CancellationToken cancellationToken = default)
    {
        var client = bot.GetRequiredService<IGraphQLClient>();
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
