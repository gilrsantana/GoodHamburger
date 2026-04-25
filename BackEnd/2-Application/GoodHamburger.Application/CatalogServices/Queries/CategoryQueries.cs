using GoodHamburger.Domain.Catalog.Enums;

namespace GoodHamburger.Application.CatalogServices.Queries;

public record GetAllCategoriesQuery(int Page = 1, int PageSize = 10);

public record GetAllCategoriesResponse(
    IReadOnlyList<CategoryDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetCategoryByIdQuery(Guid Id);

public record GetCategoryByIdResponse(CategoryDto? Category);

public record GetCategoryBySlugQuery(string Slug);

public record GetCategoryBySlugResponse(CategoryDto? Category);

public record GetCategoriesByTypeQuery(MenuCategoryType Type, int Page = 1, int PageSize = 10);

public record GetCategoriesByTypeResponse(
    IReadOnlyList<CategoryDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetActiveCategoriesQuery(int Page = 1, int PageSize = 10);

public record GetActiveCategoriesResponse(
    IReadOnlyList<CategoryDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    string Slug,
    MenuCategoryType Type,
    bool Active,
    int DisplayOrder,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
