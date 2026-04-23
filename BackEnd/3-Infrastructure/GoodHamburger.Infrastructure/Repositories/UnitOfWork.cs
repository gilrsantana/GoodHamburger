using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Domain.Repositories.Catalog;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Domain.Repositories.Ordering;
using GoodHamburger.Infrastructure.Repositories.Accounts;
using GoodHamburger.Infrastructure.Repositories.Catalog;
using GoodHamburger.Infrastructure.Repositories.Locations;
using GoodHamburger.Infrastructure.Repositories.Ordering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoodHamburger.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private IDbContextTransaction? _transaction;

    // Account Repositories
    private ICustomerProfileRepository? _customerProfiles;
    private IEmployeeProfileRepository? _employeeProfiles;
    
    // Catalog Repositories
    private ICategoryRepository? _categories;
    private IMenuItemRepository? _menuItems;
    private IIngredientRepository? _ingredients;
    private IMenuItemIngredientRepository? _menuItemIngredients;
    
    // Ordering Repositories
    private IOrderRepository? _orders;
    private IOrderItemRepository? _orderItems;
    private IOrderItemDetailRepository? _orderItemDetails;
    private ICouponRepository? _coupons;
    
    // Location Repositories
    private ICountryRepository? _countries;
    private IStateRepository? _states;
    private ICityRepository? _cities;
    private INeighborhoodRepository? _neighborhoods;
    private IStreetTypeRepository? _streetTypes;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    // Account Repositories
    public ICustomerProfileRepository CustomerProfiles => 
        _customerProfiles ??= new CustomerProfileRepository(_context);
    
    public IEmployeeProfileRepository EmployeeProfiles => 
        _employeeProfiles ??= new EmployeeProfileRepository(_context);
    
    // Catalog Repositories
    public ICategoryRepository Categories => 
        _categories ??= new CategoryRepository(_context);
    
    public IMenuItemRepository MenuItems => 
        _menuItems ??= new MenuItemRepository(_context);
    
    public IIngredientRepository Ingredients => 
        _ingredients ??= new IngredientRepository(_context);
    
    public IMenuItemIngredientRepository MenuItemIngredients => 
        _menuItemIngredients ??= new MenuItemIngredientRepository(_context);
    
    // Ordering Repositories
    public IOrderRepository Orders => 
        _orders ??= new OrderRepository(_context);
    
    public IOrderItemRepository OrderItems => 
        _orderItems ??= new OrderItemRepository(_context);
    
    public IOrderItemDetailRepository OrderItemDetails => 
        _orderItemDetails ??= new OrderItemDetailRepository(_context);
    
    public ICouponRepository Coupons => 
        _coupons ??= new CouponRepository(_context);
    
    // Location Repositories
    public ICountryRepository Countries => 
        _countries ??= new CountryRepository(_context);
    
    public IStateRepository States => 
        _states ??= new StateRepository(_context);
    
    public ICityRepository Cities => 
        _cities ??= new CityRepository(_context);
    
    public INeighborhoodRepository Neighborhoods => 
        _neighborhoods ??= new NeighborhoodRepository(_context);
    
    public IStreetTypeRepository StreetTypes => 
        _streetTypes ??= new StreetTypeRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
