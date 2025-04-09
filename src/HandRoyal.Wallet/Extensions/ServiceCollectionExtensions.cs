using HandRoyal.Wallet.Interfaces;
using HandRoyal.Wallet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HandRoyal.Wallet.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWalletServices(this IServiceCollection services)
    {
        services.AddSingleton<IWalletService, WalletService>();
        return services;
    }
}
