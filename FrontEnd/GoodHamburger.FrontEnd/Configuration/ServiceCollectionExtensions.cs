using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Fluxor;
using GoodHamburger.FrontEnd.Clients.Auth;
using GoodHamburger.FrontEnd.Clients.Categories;
using GoodHamburger.FrontEnd.Clients.Ingredients;
using GoodHamburger.FrontEnd.Clients.MenuItems;
using GoodHamburger.FrontEnd.Clients.Orders;
using GoodHamburger.FrontEnd.Clients.Coupons;
using GoodHamburger.FrontEnd.Clients.Identity;
using GoodHamburger.FrontEnd.Clients.CustomerProfiles;
using GoodHamburger.FrontEnd.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GoodHamburger.FrontEnd.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMudBlazorServices(this IServiceCollection services)
    {
        services.AddMudServices();
        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static IServiceCollection AddHttpServices(this IServiceCollection services, string apiBaseUrl)
    {
        services.AddTransient<GlobalHttpHandler>();
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(sp.GetRequiredService<IWebAssemblyHostEnvironment>().BaseAddress) });

        services.AddHttpClient<AuthClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<CategoriesClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<IngredientsClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<MenuItemsClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<OrdersClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<CouponsClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<IdentityClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();
        services.AddHttpClient<CustomerProfilesClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<GlobalHttpHandler>();

        services.AddScoped<IAuthClient>(sp => sp.GetRequiredService<AuthClient>());
        services.AddScoped<ICategoriesClient>(sp => sp.GetRequiredService<CategoriesClient>());
        services.AddScoped<IIngredientsClient>(sp => sp.GetRequiredService<IngredientsClient>());
        services.AddScoped<IMenuItemsClient>(sp => sp.GetRequiredService<MenuItemsClient>());
        services.AddScoped<IOrdersClient>(sp => sp.GetRequiredService<OrdersClient>());
        services.AddScoped<ICouponsClient>(sp => sp.GetRequiredService<CouponsClient>());
        services.AddScoped<IIdentityClient>(sp => sp.GetRequiredService<IdentityClient>());
        services.AddScoped<ICustomerProfilesClient>(sp => sp.GetRequiredService<CustomerProfilesClient>());

        return services;
    }

    public static IServiceCollection AddFluxorServices(this IServiceCollection services)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly);
        });
        return services;
    }
}
