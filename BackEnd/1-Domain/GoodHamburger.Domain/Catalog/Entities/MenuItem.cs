using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Catalog.Entities;

public class MenuItem : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string Sku { get; private set; }  = null!;
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool Active { get; private set; } = true;
    public int? Calories { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private readonly List<MenuItemIngredient> _ingredients = new();
    public IReadOnlyCollection<MenuItemIngredient> Ingredients => _ingredients;

    protected MenuItem() { }

    private MenuItem(string name, 
        string description, 
        string slug, 
        string sku, 
        decimal price, 
        Guid categoryId, 
        int? calories = null,
        string? imageUrl = null)
    {
        Name = name;
        Description = description;
        Sku = sku.ToUpper();
        Slug = slug.ToLower();
        Price = price;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        Calories = calories;

        Validate();
    }

    public static MenuItem Create(string name, 
        string description, 
        string slug, 
        string sku, 
        decimal price, 
        Guid categoryId, 
        int? calories = null,
        string? imageUrl = null)
    {
        var cal = calories is < 0 ? 0 : calories;
        var menuItem = new MenuItem(name, description, slug, sku, price < 0 ? 0 : price, categoryId, cal, imageUrl);
        return menuItem;
    }

    public MenuItem UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdateDescription(string description)
    {
        Description = description.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    public MenuItem UpdateSlug(string slug)
    {
        Slug = slug.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdateSku(string sku)
    {
        Sku = sku.Trim().ToUpper();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdatePrice(decimal price)
    {
        Price = price < 0 ? 0 : price;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdateImage(string? imageUrl)
    {
        ImageUrl = imageUrl;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdateCalories(int? calories)
    {
        Calories = calories is < 0 ? 0 : calories;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdateCategory(Category category)
    {
        Category = category;
        Validate();
        AddNotifications(Category);
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public MenuItem UpdateCategoryId(Guid categoryId)
    {
        CategoryId = categoryId;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    public void Activate()
    {
        Active = true;
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Active = false;
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvailable()
    {
        IsAvailable = true;
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    public void SetUnavailable()
    {
        IsAvailable = false;
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    public void AddIngredient(Guid ingredientId, bool isRemovable)
    {
        if (GetIngredient(ingredientId) != null) 
            return;
        
        var mii = MenuItemIngredient.Create(Id, ingredientId, isRemovable);
        AddNotifications(mii);
        _ingredients.Add(mii);
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }
    
    public void RemoveIngredient(Guid ingredientId)
    {
        var ingredient = GetIngredient(ingredientId);
        if (ingredient is null) 
            return;
        
        _ingredients.Remove(ingredient);
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateIngredientIsRemovable(Guid ingredientId, bool removable)
    {
        var ingredient = GetIngredient(ingredientId);
        if (ingredient is null) 
            return;
        
        ingredient.UpdateIsRemovable(removable);
        AddNotifications(ingredient);
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    private MenuItemIngredient? GetIngredient(Guid ingredientId)
    {
        return _ingredients.FirstOrDefault(x => x.IngredientId == ingredientId && x.MenuItemId == Id);
    }

    public void UpdateAvailability(bool available)
    {
        IsAvailable = available;
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<MenuItem>()
            .Requires()
            .IsNotNullOrEmpty(Name, "MenuItem.Name", "Nome é obrigatório.")
            .IsNotNullOrEmpty(Description, "MenuItem.Description", "Descrição é obrigatória.")  
            .IsNotNullOrEmpty(Slug, "MenuItem.Slug", "Slug é obrigatório.")  
            .IsNotNullOrEmpty(Sku, "MenuItem.Sku", "SKU é obrigatório.")
            .IsGreaterThan(Price, 0, "MenuItem.Price", "O preço deve ser maior que zero.")
            .AreNotEquals(CategoryId, Guid.Empty, "MenuItem.CategoryId", "Categoria inválida.")
            .IsUrlOrEmpty(ImageUrl, "MenuItem.ImageUrl", "URL da imagem é inválida.")
        );
    }
}