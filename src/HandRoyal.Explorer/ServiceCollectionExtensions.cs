using GraphQL.AspNet;
using GraphQL.AspNet.Configuration;
using HandRoyal.Explorer.Jwt;
using HandRoyal.Explorer.Publishers;
using HandRoyal.Explorer.ScalarTypes;
using HandRoyal.Game.Effects;
using HandRoyal.Wallet.Extensions;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace HandRoyal.Explorer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection @this)
    {
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(AddressScalarType));
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(TxIdScalarType));
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(PublicKeyScalarType));
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(PrivateKeyScalarType));
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(HexValueScalarType));
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(BlockHashScalarType));
        GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(BigIntegerScalarType));
        @this.AddWebSockets(options =>
        {
        });
        @this.AddGraphQL(options =>
        {
            options.AddAssembly(typeof(ServiceCollectionExtensions).Assembly);
            options.AddType<IEffect>();
            options.AddType<BurnEffect>();
        }).AddSubscriptions();

        @this.AddHostedService<TipEventPublisher>();
        @this.AddHostedService<SessionEventPublisher>();
        @this.AddHostedService<SessionResultEventPublisher>();
        @this.AddHostedService<UserEventPublisher>();
        @this.AddHostedService<GloveEventPublisher>();
        @this.AddHostedService<TransactionEventPublisher>();
        @this.AddHostedService<PickUpEventPublisher>();
        @this.AddHostedService<MatchMadeEventPublisher>();

        return @this;
    }

    public static IServiceCollection AddExplorerServices(
        this IServiceCollection services)
    {
        services.AddWalletServices();
        services.AddHttpContextAccessor();

        // Ensure JwtValidator is available to the Explorer
        if (!services.Any(s => s.ServiceType == typeof(JwtValidator)))
        {
            services.AddSingleton<JwtValidator>();
        }

        return services;
    }
}
