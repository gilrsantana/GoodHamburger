using GoodHamburger.Domain.Catalog.Enums;

namespace GoodHamburger.Application.CatalogServices.Commands;

public record CreateCategoryCommand(
    string Name,
    string Description,
    string Slug,
    MenuCategoryType Type,
    int DisplayOrder,
    string? ImageUrl = null
);

public record CreateCategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateCategoryCommand(
    Guid CategoryId,
    string? Name = null,
    string? Description = null,
    string? Slug = null,
    int? DisplayOrder = null,
    string? ImageUrl = null
);

public record UpdateCategoryResponse(
    Guid Id,
    string Name,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeleteCategoryCommand(Guid CategoryId);

public record DeleteCategoryResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record ActivateCategoryCommand(Guid CategoryId);

public record ActivateCategoryResponse(
    Guid Id,
    bool Active,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeactivateCategoryCommand(Guid CategoryId);

public record DeactivateCategoryResponse(
    Guid Id,
    bool Active,
    bool Succeeded,
    IEnumerable<string> Errors
);
