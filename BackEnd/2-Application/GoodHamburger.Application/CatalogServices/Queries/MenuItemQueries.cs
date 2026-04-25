namespace GoodHamburger.Application.CatalogServices.Queries;

public record GetAllMenuItemsQuery(int Page = 1, int PageSize = 10);

public record GetAllMenuItemsResponse(
    IReadOnlyList<MenuItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetMenuItemByIdQuery(Guid Id);

public record GetMenuItemByIdResponse(MenuItemDto? MenuItem);

public record GetMenuItemBySkuQuery(string Sku);

public record GetMenuItemBySkuResponse(MenuItemDto? MenuItem);

public record GetMenuItemsByCategoryQuery(Guid CategoryId, int Page = 1, int PageSize = 10);

public record GetMenuItemsByCategoryResponse(
    IReadOnlyList<MenuItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetActiveMenuItemsQuery(int Page = 1, int PageSize = 10);

public record GetActiveMenuItemsResponse(
    IReadOnlyList<MenuItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetAvailableMenuItemsQuery(int Page = 1, int PageSize = 10);

public record GetAvailableMenuItemsResponse(
    IReadOnlyList<MenuItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record SearchMenuItemsQuery(string SearchTerm, int Page = 1, int PageSize = 10);

public record SearchMenuItemsResponse(
    IReadOnlyList<MenuItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetMenuItemsByPriceRangeQuery(decimal MinPrice, decimal MaxPrice, int Page = 1, int PageSize = 10);

public record GetMenuItemsByPriceRangeResponse(
    IReadOnlyList<MenuItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record MenuItemDto(
    Guid Id,
    string Name,
    string Description,
    string Slug,
    string Sku,
    decimal Price,
    string? ImageUrl,
    bool Active,
    bool IsAvailable,
    int? Calories,
    Guid CategoryId,
    string? CategoryName,
    IReadOnlyList<MenuItemIngredientDto> Ingredients,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record MenuItemIngredientDto(
    Guid IngredientId,
    string IngredientName,
    string IngredientSku,
    bool IsRemovable
);
