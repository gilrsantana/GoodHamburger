using System.Text.Json.Serialization;

namespace GoodHamburger.FrontEnd.Clients.Categories;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MenuCategoryType { Burger, SideDish, Drink }

public class CategoryDto 
{ 
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public MenuCategoryType Type { get; set; }
    public bool Active { get; set; }
    public int DisplayOrder { get; set; }
    public string? ImageUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public record GetAllCategoriesResponse(IEnumerable<CategoryDto> Items, int TotalCount, int Page, int PageSize);
public record GetCategoryByIdResponse(CategoryDto? Category);
public record GetCategoryBySlugResponse(CategoryDto? Category);
public record GetCategoriesByTypeResponse(IEnumerable<CategoryDto> Items, int TotalCount, int Page, int PageSize);
public record GetActiveCategoriesResponse(IEnumerable<CategoryDto> Items, int TotalCount, int Page, int PageSize);
public class CreateCategoryCommand { public string Name { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public MenuCategoryType Type { get; set; } public int DisplayOrder { get; set; } public string? ImageUrl { get; set; } }
public record CreateCategoryResponse(Guid Id, string Name, string Slug, bool Succeeded, IEnumerable<string>? Errors);
public class UpdateCategoryCommand { public Guid CategoryId { get; set; } public string? Name { get; set; } public string? Description { get; set; } public string? Slug { get; set; } public int? DisplayOrder { get; set; } public string? ImageUrl { get; set; } }
public record UpdateCategoryResponse(Guid Id, string Name, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteCategoryResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
public record ActivateCategoryResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record DeactivateCategoryResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
