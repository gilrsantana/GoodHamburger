namespace GoodHamburger.Application.CatalogServices.Queries;

public record GetAllIngredientsQuery(int Page = 1, int PageSize = 10);

public record GetAllIngredientsResponse(
    IReadOnlyList<IngredientDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetIngredientByIdQuery(Guid Id);

public record GetIngredientByIdResponse(IngredientDto? Ingredient);

public record GetActiveIngredientsQuery(int Page = 1, int PageSize = 10);

public record GetActiveIngredientsResponse(
    IReadOnlyList<IngredientDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record SearchIngredientsQuery(string SearchTerm, int Page = 1, int PageSize = 10);

public record SearchIngredientsResponse(
    IReadOnlyList<IngredientDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetIngredientsByPriceRangeQuery(decimal MinPrice, decimal MaxPrice, int Page = 1, int PageSize = 10);

public record GetIngredientsByPriceRangeResponse(
    IReadOnlyList<IngredientDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record IngredientDto(
    Guid Id,
    string Name,
    string Sku,
    bool Active,
    decimal SalePrice,
    decimal ReferenceCostPrice,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
