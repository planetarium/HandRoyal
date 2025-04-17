using HandRoyal.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HandRoyal.Bot.Pages;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBotPages(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<BotPageViewModel>()
            .AddSingleton<IPage>(s => s.GetRequiredService<BotPageViewModel>());

        return services;
    }
}
