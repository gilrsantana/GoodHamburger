namespace GoodHamburger.Application.CatalogServices.Commands;

public record CreateMenuItemCommand(
    string Name,
    string Description,
    string Slug,
    string Sku,
    decimal Price,
    Guid CategoryId,
    int? Calories = null,
    string? ImageUrl = null
);

public record CreateMenuItemResponse(
    Guid Id,
    string Name,
    string Sku,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateMenuItemCommand(
    Guid MenuItemId,
    string? Name = null,
    string? Description = null,
    string? Slug = null,
    string? Sku = null,
    decimal? Price = null,
    Guid? CategoryId = null,
    int? Calories = null,
    string? ImageUrl = null
);

public record UpdateMenuItemResponse(
    Guid Id,
    string Name,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeleteMenuItemCommand(Guid MenuItemId);

public record DeleteMenuItemResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record ActivateMenuItemCommand(Guid MenuItemId);

public record ActivateMenuItemResponse(
    Guid Id,
    bool Active,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeactivateMenuItemCommand(Guid MenuItemId);

public record DeactivateMenuItemResponse(
    Guid Id,
    bool Active,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record SetMenuItemAvailabilityCommand(Guid MenuItemId, bool IsAvailable);

public record SetMenuItemAvailabilityResponse(
    Guid Id,
    bool IsAvailable,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record AddIngredientToMenuItemCommand(
    Guid MenuItemId,
    Guid IngredientId,
    bool IsRemovable
);

public record AddIngredientToMenuItemResponse(
    Guid MenuItemId,
    Guid IngredientId,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record RemoveIngredientFromMenuItemCommand(
    Guid MenuItemId,
    Guid IngredientId
);

public record RemoveIngredientFromMenuItemResponse(
    Guid MenuItemId,
    Guid IngredientId,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateIngredientRemovabilityCommand(
    Guid MenuItemId,
    Guid IngredientId,
    bool IsRemovable
);

public record UpdateIngredientRemovabilityResponse(
    Guid MenuItemId,
    Guid IngredientId,
    bool IsRemovable,
    bool Succeeded,
    IEnumerable<string> Errors
);
