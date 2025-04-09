using HandRoyal.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HandRoyal.BotPages;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBotPages(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPage, Bot>();

        return services;
    }
}
