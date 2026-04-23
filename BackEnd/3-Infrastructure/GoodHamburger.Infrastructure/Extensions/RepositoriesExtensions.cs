using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Domain.Repositories.Catalog;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Domain.Repositories.Ordering;
using GoodHamburger.Infrastructure.Repositories;
using GoodHamburger.Infrastructure.Repositories.Accounts;
using GoodHamburger.Infrastructure.Repositories.Catalog;
using GoodHamburger.Infrastructure.Repositories.Locations;
using GoodHamburger.Infrastructure.Repositories.Ordering;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Infrastructure.Extensions;

public static class RepositoriesExtensions
{
    public static IServiceCollection AddRepositoriesConfiguration(this IServiceCollection services)
    {
        // Ordering repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderItemDetailRepository, OrderItemDetailRepository>();

        // Catalog repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IMenuItemIngredientRepository, MenuItemIngredientRepository>();

        // Location repositories
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<INeighborhoodRepository, NeighborhoodRepository>();
        services.AddScoped<IStreetTypeRepository, StreetTypeRepository>();

        // Account repositories
        services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
        services.AddScoped<IEmployeeProfileRepository, EmployeeProfileRepository>();
        
        // Base repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        
        return services;
    }
}