using GoodHamburger.Shared.Handlers;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.CatalogServices.Queries;

public interface IGetAllMenuItemsHandler
{
    Task<Result<GetAllMenuItemsResponse>> HandleAsync(GetAllMenuItemsQuery query, CancellationToken cancellationToken = default);
}

public interface IGetMenuItemByIdHandler
{
    Task<Result<GetMenuItemByIdResponse>> HandleAsync(GetMenuItemByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetMenuItemBySkuHandler
{
    Task<Result<GetMenuItemBySkuResponse>> HandleAsync(GetMenuItemBySkuQuery query, CancellationToken cancellationToken = default);
}

public interface IGetMenuItemsByCategoryHandler
{
    Task<Result<GetMenuItemsByCategoryResponse>> HandleAsync(GetMenuItemsByCategoryQuery query, CancellationToken cancellationToken = default);
}

public interface IGetActiveMenuItemsHandler
{
    Task<Result<GetActiveMenuItemsResponse>> HandleAsync(GetActiveMenuItemsQuery query, CancellationToken cancellationToken = default);
}

public interface IGetAvailableMenuItemsHandler
{
    Task<Result<GetAvailableMenuItemsResponse>> HandleAsync(GetAvailableMenuItemsQuery query, CancellationToken cancellationToken = default);
}

public interface ISearchMenuItemsHandler
{
    Task<Result<SearchMenuItemsResponse>> HandleAsync(SearchMenuItemsQuery query, CancellationToken cancellationToken = default);
}

public interface IGetMenuItemsByPriceRangeHandler
{
    Task<Result<GetMenuItemsByPriceRangeResponse>> HandleAsync(GetMenuItemsByPriceRangeQuery query, CancellationToken cancellationToken = default);
}

public class GetAllMenuItemsHandler : IGetAllMenuItemsHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetAllMenuItemsHandler> _logger;

    public GetAllMenuItemsHandler(IMenuItemRepository menuItemRepository, ILogger<GetAllMenuItemsHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllMenuItemsResponse>> HandleAsync(GetAllMenuItemsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetAllAsync(cancellationToken);
            
            var totalCount = menuItems.Count;
            var paginatedItems = menuItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MenuItemDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Slug,
                    m.Sku,
                    m.Price,
                    m.ImageUrl,
                    m.Active,
                    m.IsAvailable,
                    m.Calories,
                    m.CategoryId,
                    m.Category?.Name,
                    m.Ingredients.Select(i => new MenuItemIngredientDto(
                        i.IngredientId,
                        i.Ingredient?.Name ?? string.Empty,
                        i.Ingredient?.Sku ?? string.Empty,
                        i.IsRemovable
                    )).ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ))
                .ToList();

            return Result<GetAllMenuItemsResponse>.Success(new GetAllMenuItemsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all menu items");
            return Result<GetAllMenuItemsResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving menu items")
            );
        }
    }
}

public class GetMenuItemByIdHandler : IGetMenuItemByIdHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetMenuItemByIdHandler> _logger;

    public GetMenuItemByIdHandler(IMenuItemRepository menuItemRepository, ILogger<GetMenuItemByIdHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetMenuItemByIdResponse>> HandleAsync(GetMenuItemByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(query.Id, cancellationToken);
            
            if (menuItem == null)
            {
                return Result<GetMenuItemByIdResponse>.Success(new GetMenuItemByIdResponse(null));
            }

            var dto = new MenuItemDto(
                menuItem.Id,
                menuItem.Name,
                menuItem.Description,
                menuItem.Slug,
                menuItem.Sku,
                menuItem.Price,
                menuItem.ImageUrl,
                menuItem.Active,
                menuItem.IsAvailable,
                menuItem.Calories,
                menuItem.CategoryId,
                menuItem.Category?.Name,
                menuItem.Ingredients.Select(i => new MenuItemIngredientDto(
                    i.IngredientId,
                    i.Ingredient?.Name ?? string.Empty,
                    i.Ingredient?.Sku ?? string.Empty,
                    i.IsRemovable
                )).ToList(),
                menuItem.CreatedAt,
                menuItem.UpdatedAt
            );

            return Result<GetMenuItemByIdResponse>.Success(new GetMenuItemByIdResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting menu item by ID {MenuItemId}", query.Id);
            return Result<GetMenuItemByIdResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving the menu item")
            );
        }
    }
}

public class GetMenuItemBySkuHandler : IGetMenuItemBySkuHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetMenuItemBySkuHandler> _logger;

    public GetMenuItemBySkuHandler(IMenuItemRepository menuItemRepository, ILogger<GetMenuItemBySkuHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetMenuItemBySkuResponse>> HandleAsync(GetMenuItemBySkuQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetBySkuAsync(query.Sku, cancellationToken);
            
            if (menuItem == null)
            {
                return Result<GetMenuItemBySkuResponse>.Success(new GetMenuItemBySkuResponse(null));
            }

            var dto = new MenuItemDto(
                menuItem.Id,
                menuItem.Name,
                menuItem.Description,
                menuItem.Slug,
                menuItem.Sku,
                menuItem.Price,
                menuItem.ImageUrl,
                menuItem.Active,
                menuItem.IsAvailable,
                menuItem.Calories,
                menuItem.CategoryId,
                menuItem.Category?.Name,
                menuItem.Ingredients.Select(i => new MenuItemIngredientDto(
                    i.IngredientId,
                    i.Ingredient?.Name ?? string.Empty,
                    i.Ingredient?.Sku ?? string.Empty,
                    i.IsRemovable
                )).ToList(),
                menuItem.CreatedAt,
                menuItem.UpdatedAt
            );

            return Result<GetMenuItemBySkuResponse>.Success(new GetMenuItemBySkuResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting menu item by SKU {Sku}", query.Sku);
            return Result<GetMenuItemBySkuResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving the menu item")
            );
        }
    }
}

public class GetMenuItemsByCategoryHandler : IGetMenuItemsByCategoryHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetMenuItemsByCategoryHandler> _logger;

    public GetMenuItemsByCategoryHandler(IMenuItemRepository menuItemRepository, ILogger<GetMenuItemsByCategoryHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetMenuItemsByCategoryResponse>> HandleAsync(GetMenuItemsByCategoryQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetByCategoryAsync(query.CategoryId, cancellationToken);
            
            var totalCount = menuItems.Count;
            var paginatedItems = menuItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MenuItemDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Slug,
                    m.Sku,
                    m.Price,
                    m.ImageUrl,
                    m.Active,
                    m.IsAvailable,
                    m.Calories,
                    m.CategoryId,
                    m.Category?.Name,
                    m.Ingredients.Select(i => new MenuItemIngredientDto(
                        i.IngredientId,
                        i.Ingredient?.Name ?? string.Empty,
                        i.Ingredient?.Sku ?? string.Empty,
                        i.IsRemovable
                    )).ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ))
                .ToList();

            return Result<GetMenuItemsByCategoryResponse>.Success(new GetMenuItemsByCategoryResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting menu items by category {CategoryId}", query.CategoryId);
            return Result<GetMenuItemsByCategoryResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving menu items")
            );
        }
    }
}

public class GetActiveMenuItemsHandler : IGetActiveMenuItemsHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetActiveMenuItemsHandler> _logger;

    public GetActiveMenuItemsHandler(IMenuItemRepository menuItemRepository, ILogger<GetActiveMenuItemsHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetActiveMenuItemsResponse>> HandleAsync(GetActiveMenuItemsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetActiveItemsAsync(cancellationToken);
            
            var totalCount = menuItems.Count;
            var paginatedItems = menuItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MenuItemDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Slug,
                    m.Sku,
                    m.Price,
                    m.ImageUrl,
                    m.Active,
                    m.IsAvailable,
                    m.Calories,
                    m.CategoryId,
                    m.Category?.Name,
                    m.Ingredients.Select(i => new MenuItemIngredientDto(
                        i.IngredientId,
                        i.Ingredient?.Name ?? string.Empty,
                        i.Ingredient?.Sku ?? string.Empty,
                        i.IsRemovable
                    )).ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ))
                .ToList();

            return Result<GetActiveMenuItemsResponse>.Success(new GetActiveMenuItemsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting active menu items");
            return Result<GetActiveMenuItemsResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving menu items")
            );
        }
    }
}

public class GetAvailableMenuItemsHandler : IGetAvailableMenuItemsHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetAvailableMenuItemsHandler> _logger;

    public GetAvailableMenuItemsHandler(IMenuItemRepository menuItemRepository, ILogger<GetAvailableMenuItemsHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetAvailableMenuItemsResponse>> HandleAsync(GetAvailableMenuItemsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetAvailableItemsAsync(cancellationToken);
            
            var totalCount = menuItems.Count;
            var paginatedItems = menuItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MenuItemDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Slug,
                    m.Sku,
                    m.Price,
                    m.ImageUrl,
                    m.Active,
                    m.IsAvailable,
                    m.Calories,
                    m.CategoryId,
                    m.Category?.Name,
                    m.Ingredients.Select(i => new MenuItemIngredientDto(
                        i.IngredientId,
                        i.Ingredient?.Name ?? string.Empty,
                        i.Ingredient?.Sku ?? string.Empty,
                        i.IsRemovable
                    )).ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ))
                .ToList();

            return Result<GetAvailableMenuItemsResponse>.Success(new GetAvailableMenuItemsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting available menu items");
            return Result<GetAvailableMenuItemsResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving menu items")
            );
        }
    }
}

public class SearchMenuItemsHandler : ISearchMenuItemsHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<SearchMenuItemsHandler> _logger;

    public SearchMenuItemsHandler(IMenuItemRepository menuItemRepository, ILogger<SearchMenuItemsHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<SearchMenuItemsResponse>> HandleAsync(SearchMenuItemsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItems = await _menuItemRepository.SearchByNameAsync(query.SearchTerm, cancellationToken);
            
            var totalCount = menuItems.Count;
            var paginatedItems = menuItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MenuItemDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Slug,
                    m.Sku,
                    m.Price,
                    m.ImageUrl,
                    m.Active,
                    m.IsAvailable,
                    m.Calories,
                    m.CategoryId,
                    m.Category?.Name,
                    m.Ingredients.Select(i => new MenuItemIngredientDto(
                        i.IngredientId,
                        i.Ingredient?.Name ?? string.Empty,
                        i.Ingredient?.Sku ?? string.Empty,
                        i.IsRemovable
                    )).ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ))
                .ToList();

            return Result<SearchMenuItemsResponse>.Success(new SearchMenuItemsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching menu items with term {SearchTerm}", query.SearchTerm);
            return Result<SearchMenuItemsResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while searching menu items")
            );
        }
    }
}

public class GetMenuItemsByPriceRangeHandler : IGetMenuItemsByPriceRangeHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<GetMenuItemsByPriceRangeHandler> _logger;

    public GetMenuItemsByPriceRangeHandler(IMenuItemRepository menuItemRepository, ILogger<GetMenuItemsByPriceRangeHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetMenuItemsByPriceRangeResponse>> HandleAsync(GetMenuItemsByPriceRangeQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetItemsByPriceRangeAsync(query.MinPrice, query.MaxPrice, cancellationToken);
            
            var totalCount = menuItems.Count;
            var paginatedItems = menuItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MenuItemDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Slug,
                    m.Sku,
                    m.Price,
                    m.ImageUrl,
                    m.Active,
                    m.IsAvailable,
                    m.Calories,
                    m.CategoryId,
                    m.Category?.Name,
                    m.Ingredients.Select(i => new MenuItemIngredientDto(
                        i.IngredientId,
                        i.Ingredient?.Name ?? string.Empty,
                        i.Ingredient?.Sku ?? string.Empty,
                        i.IsRemovable
                    )).ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ))
                .ToList();

            return Result<GetMenuItemsByPriceRangeResponse>.Success(new GetMenuItemsByPriceRangeResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting menu items by price range {MinPrice}-{MaxPrice}", query.MinPrice, query.MaxPrice);
            return Result<GetMenuItemsByPriceRangeResponse>.Failure(
                new Error("MenuItem.QueryError", "An error occurred while retrieving menu items")
            );
        }
    }
}
