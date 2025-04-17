using HandRoyal.Bot;
using HandRoyal.Bot.Pages;
using HandRoyal.Executable;
using HandRoyal.Executable.Data;
using HandRoyal.Executable.Logging;
using HandRoyal.Executable.Pages;
using HandRoyal.Explorer;
using HandRoyal.Explorer.Jwt;
using HandRoyal.Pages;
using Libplanet.Node.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MudBlazor.Services;
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

builder.Services.AddBotPages(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<ThemeService>()
    .AddSingleton<IThemeService>(s => s.GetRequiredService<ThemeService>());

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<SettingsSchemaService>();
builder.Services.AddSingleton<IPage, SchemaPage>();
builder.Services.AddSingleton<IPage, CounterPage>();
builder.Services.AddSingleton<IPage, WeatherPage>();

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
builder.Services.AddBot(builder.Configuration);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService().AllowAnonymous();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors("AllowAll");
app.UseExplorer();
app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Map Blazor endpoints
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
