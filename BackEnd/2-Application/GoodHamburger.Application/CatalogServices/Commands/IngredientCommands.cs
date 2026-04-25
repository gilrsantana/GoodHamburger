namespace GoodHamburger.Application.CatalogServices.Commands;

public record CreateIngredientCommand(
    string Name,
    string Sku,
    decimal ReferenceCostPrice,
    decimal SalePrice
);

public record CreateIngredientResponse(
    Guid Id,
    string Name,
    string Sku,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateIngredientCommand(
    Guid IngredientId,
    string? Name = null,
    string? Sku = null,
    decimal? ReferenceCostPrice = null,
    decimal? SalePrice = null
);

public record UpdateIngredientResponse(
    Guid Id,
    string Name,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeleteIngredientCommand(Guid IngredientId);

public record DeleteIngredientResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record ActivateIngredientCommand(Guid IngredientId);

public record ActivateIngredientResponse(
    Guid Id,
    bool Active,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeactivateIngredientCommand(Guid IngredientId);

public record DeactivateIngredientResponse(
    Guid Id,
    bool Active,
    bool Succeeded,
    IEnumerable<string> Errors
);
