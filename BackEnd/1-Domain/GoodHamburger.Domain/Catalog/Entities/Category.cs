using Flunt.Validations;
using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Catalog.Entities;

public class Category : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public MenuCategoryType Type { get; private set; }
    public bool Active { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public string? ImageUrl { get; private set; }
    
    private readonly List<MenuItem> _items = new();
    public IReadOnlyCollection<MenuItem> Items => _items;
    
    protected Category() { }

    private Category(
        string name, 
        string description, 
        string slug, 
        MenuCategoryType type, 
        int displayOrder, 
        string? imageUrl = null)
    {
        Name = name;
        Description = description;
        Slug = slug.ToLower();
        Type = type;
        DisplayOrder = displayOrder;
        ImageUrl = imageUrl;
        
        Validate();
    }

    public static Category Create(string name, 
        string description, 
        string slug, 
        MenuCategoryType type, 
        int displayOrder, 
        string? imageUrl = null)
    {
        var order = displayOrder < 0 ? 0 : displayOrder;
        var category = new Category(name.Trim(), description.Trim(), slug.Trim(), type, order, imageUrl);
        return category;
    }

    public Category UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Category UpdateDescription(string description)
    {
        Description = description.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    

    public Category UpdateSlug(string slug)
    {
        Slug = slug.Trim().ToLower();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    public Category UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    public Category UpdateImage(string? imageUrl)
    {
        ImageUrl = imageUrl;
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

    public sealed override void Validate()
    {
        AddNotifications(new Contract<Category>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Category.Name", "Nome da Categoria é obrigatório")
            .IsNotNullOrEmpty(Description, "Category.Description", "Dscrição da Categoria é obrigatório.")
            .IsNotNullOrEmpty(Slug, "Category.Slug", "Apelido da Categoria é obrigatório.")
            .IsUrlOrEmpty(ImageUrl, "Category.ImageUrl", "URL da imagem é inválida.")
        );
    }
}