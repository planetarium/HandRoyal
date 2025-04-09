using HandRoyal.Bot;
using HandRoyal.Bot.Pages;
using HandRoyal.Explorer;
using HandRoyal.Node.Data;
using HandRoyal.Node.Logging;
using HandRoyal.Node.Pages;
using HandRoyal.Pages;
using Libplanet.Node.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication("Bearer").AddJwtBearer();
}

if (Environment.GetEnvironmentVariable("APPSETTINGS_PATH") is { } appSettingsPath)
{
    builder.Configuration.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);
}

// Register services from HandRoyal.Bot.Pages
builder.Services.AddBotPages(builder.Configuration);

// Add services for Razor components and Blazor server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<SettingsSchemaService>();
builder.Services.AddTransient<IPage, Counter>();
builder.Services.AddTransient<IPage, Weather>();
builder.Services.AddTransient<IPage, Schema>();

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
builder.Services.AddBot(builder.Configuration);
builder.Services.AddExplorerPages();

builder.Services.AddControllers();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService().AllowAnonymous();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
