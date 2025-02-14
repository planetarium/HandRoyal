using GraphQL.AspNet;
using GraphQL.AspNet.Configuration;
using HandRoyal.Explorer.Publishers;
using HandRoyal.Explorer.ScalarTypes;
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
        }).AddSubscriptions();

        @this.AddHostedService<BlockChainRendererEventPublisher>();
        @this.AddHostedService<SubmitMoveRendererEventPublisher>();

        return @this;
    }
}
