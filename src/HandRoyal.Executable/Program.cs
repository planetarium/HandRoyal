using HandRoyal.Executable;
using HandRoyal.Executable.Logging;
using HandRoyal.Explorer;
using HandRoyal.Explorer.Jwt;
using Libplanet.Node.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.AddConsole();
builder.Logging.AddProvider(new DefaultLoggerToSerilogSink((ILogEventSink)Log.Logger));

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5259, o => o.Protocols =
            HttpProtocols.Http1AndHttp2);
        options.ListenLocalhost(5260, o => o.Protocols =
            HttpProtocols.Http2);
    });
}

if (Environment.GetEnvironmentVariable("APPSETTINGS_PATH") is { } appSettingsPath)
{
    builder.Configuration.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);
}

// Register Supabase options
builder.Services.Configure<SupabaseOptions>(
    builder.Configuration.GetSection("Supabase"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", new CorsPolicyBuilder()
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .Build());
});

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddLibplanetNode(builder.Configuration);
builder.Services.AddExplorer();
builder.Services.AddExplorerServices();
builder.Services.AddHostedService<BlockChainRendererTracer>();
builder.Services.AddControllers();
builder.Services.AddDirectoryBrowser(); // Optional: Enable directory browsing for static files
builder.Services.AddHttpContextAccessor();

var handlerMessage = """
    Communication with gRPC endpoints must be made through a gRPC client. To learn how to
    create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909
    """;
using var app = builder.Build();

// Serve static files from the wwwroot folder
app.UseStaticFiles();
app.UseDirectoryBrowser(); // Optional: Enable directory browsing

app.MapGet("/", () => handlerMessage);
if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService().AllowAnonymous();
}

// Use GraphQL middleware
app.UseCors("AllowAll");
app.UseExplorer();

app.MapSchemaBuilder("/v1/schema");
app.MapGet("/schema", context => Task.Run(() => context.Response.Redirect("/v1/schema")));
app.MapControllers();

await app.RunAsync();
