using HandRoyal.Bot.GraphQL.Serializers;
using HandRoyal.Bot.Jobs;
using HandRoyal.Bot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HandRoyal.Bot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBot(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<BotServiceOptions>()
            .Bind(configuration.GetSection(BotServiceOptions.Position));

        services.AddSingleton<
            IConfigureOptions<BotServiceOptions>, BotServiceOptionsConfigurator>();
        services.AddSingleton<IValidateOptions<BotServiceOptions>, BotServiceOptionsValidator>();

        services.AddJob<CancelMatchingJob>();
        services.AddJob<CreateSessionJob>();
        services.AddJob<CreateUserJob>();
        services.AddJob<IdleJob>();
        services.AddJob<JoinSessionJob>();
        services.AddJob<PickUpJob>();
        services.AddJob<PickUpManyJob>();
        services.AddJob<RegisterMatchingJob>();
        services.AddJob<SessionJob>();
        services.AddJob<SubmitMoveJob>();
        services.AddJob<UpdateUserJob>();
        services.AddJob<WaitBlockJob>();
        services.AddJob<WaitMatchingJob>();
        services.AddJob<WaitSessionJob>();

        services.AddSingleton<JobService>()
            .AddSingleton<IJobService>(s => s.GetRequiredService<JobService>())
            .AddHostedService(s => s.GetRequiredService<JobService>());

        if (IsBotEnabled(configuration))
        {
            services.AddSingleton<BotService>()
                .AddSingleton<IBotService>(s => s.GetRequiredService<BotService>())
                .AddHostedService(s => s.GetRequiredService<BotService>());
            services.AddGraphQLClient()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = GetHttpEndpoint(configuration);
                })
                .ConfigureWebSocketClient(client =>
                {
                    client.Uri = GetWebSocketEndpoint(configuration);
                });
            services.AddSerializer<AddressSerializer>();
            services.AddSerializer<TxIdSerializer>();
        }

        return services;
    }

    private static bool IsBotEnabled(IConfiguration configuration)
        => configuration.GetValue<bool>($"{BotServiceOptions.Position}:IsEnabled");

    private static Uri GetHttpEndpoint(IConfiguration configuration)
    {
        var key = $"{BotServiceOptions.Position}:GraphqlEndpoint";
        if (configuration.GetValue<string>(key) is not { } endPoint)
        {
            throw new InvalidOperationException("GraphQL endpoint is not configured.");
        }

        return new Uri(endPoint);
    }

    private static Uri GetWebSocketEndpoint(IConfiguration configuration)
    {
        var key = $"{BotServiceOptions.Position}:GraphqlEndpoint";
        if (configuration.GetValue<string>(key) is not { } endPoint)
        {
            throw new InvalidOperationException("GraphQL endpoint is not configured.");
        }

        var uriBuilder = new UriBuilder(endPoint)
        {
            Path = "/graphql",
        };
        uriBuilder.Scheme = uriBuilder.Scheme switch
        {
            "http" => "ws",
            "https" => "wss",
            _ => throw new NotSupportedException($"Unsupported scheme: {uriBuilder.Scheme}"),
        };

        return uriBuilder.Uri;
    }

    private static IServiceCollection AddJob<TJob>(
        this IServiceCollection services)
        where TJob : class, IJob
    {
        return services.AddSingleton<TJob>()
            .AddSingleton<IJob>(s => s.GetRequiredService<TJob>());
    }
}
