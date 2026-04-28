namespace GoodHamburger.FrontEnd.Clients.MenuItems;

public record MenuItemIngredientDto(Guid IngredientId, string IngredientName, string IngredientSku, bool IsRemovable);

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public double? Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool Active { get; set; }
    public bool IsAvailable { get; set; }
    public int? Calories { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public IEnumerable<MenuItemIngredientDto> Ingredients { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public record GetAllMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetMenuItemByIdResponse(MenuItemDto? MenuItem);
public record GetMenuItemBySkuResponse(MenuItemDto? MenuItem);
public record GetMenuItemsByCategoryResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetActiveMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetAvailableMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record SearchMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetMenuItemsByPriceRangeResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public class CreateMenuItemCommand { public string Name { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public string Sku { get; set; } = string.Empty; public double Price { get; set; } public Guid CategoryId { get; set; } public int? Calories { get; set; } public string? ImageUrl { get; set; } }
public record CreateMenuItemResponse(Guid Id, string Name, string Sku, bool Succeeded, IEnumerable<string>? Errors);
public class UpdateMenuItemCommand { public Guid MenuItemId { get; set; } public string? Name { get; set; } public string? Description { get; set; } public string? Slug { get; set; } public string? Sku { get; set; } public double? Price { get; set; } public Guid? CategoryId { get; set; } public int? Calories { get; set; } public string? ImageUrl { get; set; } }
public record UpdateMenuItemResponse(Guid Id, string Name, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteMenuItemResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
public record ActivateMenuItemResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record DeactivateMenuItemResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record SetMenuItemAvailabilityResponse(Guid Id, bool IsAvailable, bool Succeeded, IEnumerable<string>? Errors);
public record AddIngredientToMenuItemResponse(bool Succeeded, IEnumerable<string>? Errors);
public record RemoveIngredientFromMenuItemResponse(bool Succeeded, IEnumerable<string>? Errors);
