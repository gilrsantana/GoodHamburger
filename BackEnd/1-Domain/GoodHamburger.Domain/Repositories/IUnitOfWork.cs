using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Domain.Repositories.Catalog;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Domain.Repositories.Ordering;

namespace GoodHamburger.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    // Account Repositories
    ICustomerProfileRepository CustomerProfiles { get; }
    IEmployeeProfileRepository EmployeeProfiles { get; }
    
    // Catalog Repositories
    ICategoryRepository Categories { get; }
    IMenuItemRepository MenuItems { get; }
    IIngredientRepository Ingredients { get; }
    IMenuItemIngredientRepository MenuItemIngredients { get; }
    
    // Ordering Repositories
    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }
    IOrderItemDetailRepository OrderItemDetails { get; }
    ICouponRepository Coupons { get; }
    
    // Location Repositories
    ICountryRepository Countries { get; }
    IStateRepository States { get; }
    ICityRepository Cities { get; }
    INeighborhoodRepository Neighborhoods { get; }
    IStreetTypeRepository StreetTypes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
