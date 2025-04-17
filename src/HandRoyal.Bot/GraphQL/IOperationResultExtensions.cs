using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using StrawberryShake;
using StrawberryShake.Extensions;

namespace HandRoyal.Bot.GraphQL;

internal static class IOperationResultExtensions
{
    public static TResult As<TResultData, TResult>(
        this IOperationResult<TResultData> @this, Func<TResultData, TResult?> func)
        where TResultData : class
        where TResult : notnull
    {
        if (@this.Data is not { } data)
        {
            throw new InvalidOperationException("Data is null");
        }

        var value = func(data) ?? throw new InvalidOperationException("Value is null");
        return value;
    }

    public static async Task<TResult> ResolveAsync<TResultData, TResult>(
        this Task<IOperationResult<TResultData>> @this, Func<TResultData, TResult?> func)
        where TResultData : class
        where TResult : notnull
    {
        var result = await @this;
        return result.As(func);
    }

    public static async IAsyncEnumerable<TResult> AsAsyncEnumerableAsync<TResultData, TResult>(
        this IObservable<IOperationResult<TResultData>> @this,
        Func<TResultData, TResult?> func,
        [EnumeratorCancellation] CancellationToken cancellationToken)
        where TResultData : class
        where TResult : notnull
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        var observer = new SubscriptionObserver<TResultData, TResult>(
            func, cancellationTokenSource.Token);
        var scope = @this.Subscribe(observer);
        using var registration = cancellationToken.Register(scope.Dispose);
        await foreach (var item in observer.AsAsyncEnumerable())
        {
            yield return item;
        }
    }

    private sealed class SubscriptionObserver<TResultData, TResult>(
        Func<TResultData, TResult?> func, CancellationToken cancellationToken)
        : IObserver<IOperationResult<TResultData>>
        where TResultData : class
        where TResult : notnull
    {
        private readonly Channel<TResult> _channel = Channel.CreateUnbounded<TResult>();

        public void OnCompleted() => _channel.Writer.Complete();

        public void OnError(Exception error) => _channel.Writer.Complete(error);

        public void OnNext(IOperationResult<TResultData> value)
        {
            if (value.Data is not null && func(value.Data) is { } data)
            {
                _channel.Writer.TryWrite(data);
            }
        }

        internal async IAsyncEnumerable<TResult> AsAsyncEnumerable()
        {
            await foreach (var data in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return data;
            }
        }
    }
}
