using GraphQL.AspNet.Configuration;
using Microsoft.AspNetCore.Builder;

namespace HandRoyal.Explorer;

public static class NodeEndpointRouteBuilderExtensions
{
    public static WebApplication UseExplorer(this WebApplication @this)
    {
        @this.UseWebSockets();
        @this.UseGraphQL();

        return @this;
    }
}
