using GoodHamburger.Application.Services.Ordering.Contracts;
using GoodHamburger.Application.Services.Ordering.Helper;
using GoodHamburger.Application.Services.Ordering.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Application.Extensions;

public static class ApplicationConfigurationExtensions
{
    public static IServiceCollection AddApplicationServicesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IOrderDiscountService, CategoryDiscountService>();
        services.AddScoped<HashSetEqualityComparer>();
        return services;
    }
}