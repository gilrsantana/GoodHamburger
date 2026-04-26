using GoodHamburger.ExternalProviders.Address;
using GoodHamburger.ExternalProviders.Address.ViaCep;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.ExternalProviders.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalProviders(this IServiceCollection services)
    {
        services.AddHttpClient<IViaCepService, ViaCepService>();
        return services;
    }
}
