using GraphQL.AspNet;
using GraphQL.AspNet.Configuration;
using HandRoyal.Node;
using HandRoyal.Node.Explorer.Types;
using Libplanet.Node.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Setup a HTTP/2 endpoint without TLS.
        options.ListenLocalhost(5259, o => o.Protocols =
            HttpProtocols.Http1AndHttp2);
        options.ListenLocalhost(5260, o => o.Protocols =
            HttpProtocols.Http2);
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication("Bearer").AddJwtBearer();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", new CorsPolicyBuilder()
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .Build());
});
GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(AddressScalarType));
GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(TxIdScalarType));
GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(PublicKeyScalarType));
GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(PrivateKeyScalarType));
GraphQLProviders.ScalarProvider.RegisterCustomScalar(typeof(HexValueScalarType));
builder.Services.AddGraphQL();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddLibplanetNode(builder.Configuration);
builder.Services.AddHostedService<BlockChainRendererTracer>();

var handlerMessage = """
    Communication with gRPC endpoints must be made through a gRPC client. To learn how to
    create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909
    """;
using var app = builder.Build();

app.MapGet("/", () => handlerMessage);
if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService().AllowAnonymous();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use GraphQL middleware
app.UseCors("AllowAll");
app.UseGraphQL();

app.MapSchemaBuilder("/v1/schema");
app.MapGet("/schema", context => Task.Run(() => context.Response.Redirect("/v1/schema")));

await app.RunAsync();
