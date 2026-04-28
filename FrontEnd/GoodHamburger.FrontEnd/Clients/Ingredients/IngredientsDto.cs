namespace GoodHamburger.FrontEnd.Clients.Ingredients;

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public bool Active { get; set; }
    public double SalePrice { get; set; }
    public double ReferenceCostPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public record GetAllIngredientsResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public record GetIngredientByIdResponse(IngredientDto? Ingredient);
public record GetActiveIngredientsResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public record SearchIngredientsResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public record GetIngredientsByPriceRangeResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public class CreateIngredientCommand { public string Name { get; set; } = string.Empty; public string Sku { get; set; } = string.Empty; public double ReferenceCostPrice { get; set; } public double SalePrice { get; set; } }
public record CreateIngredientResponse(Guid Id, string Name, string Sku, bool Succeeded, IEnumerable<string>? Errors);
public class UpdateIngredientCommand { public Guid IngredientId { get; set; } public string? Name { get; set; } public string? Sku { get; set; } public double? ReferenceCostPrice { get; set; } public double? SalePrice { get; set; } }
public record UpdateIngredientResponse(Guid Id, string Name, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteIngredientResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
public record ActivateIngredientResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record DeactivateIngredientResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
