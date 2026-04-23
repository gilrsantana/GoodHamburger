using Flunt.Notifications;
using Flunt.Validations;

namespace GoodHamburger.Domain.Catalog.Entities;

public class MenuItemIngredient : Notifiable<Notification>
{
    public Guid MenuItemId { get; private set; }
    public MenuItem? MenuItem { get; private set; }
    public Guid IngredientId { get; private set; }
    public Ingredient? Ingredient { get; private set; }
    public bool IsRemovable { get; private set; }

    protected MenuItemIngredient() { }

    private MenuItemIngredient(Guid menuItemId, Guid ingredientId, bool isRemovable)
    {
        MenuItemId = menuItemId;
        IngredientId = ingredientId;
        IsRemovable = isRemovable;
        Validate();
    }
    
    internal static MenuItemIngredient Create(Guid menuItemId, Guid ingredientId, bool isRemovable)
    {
        var ingredient = new MenuItemIngredient(menuItemId, ingredientId, isRemovable);
        return ingredient;
    }
    
    internal void UpdateIsRemovable(bool isRemovable)
    {
        IsRemovable = isRemovable;
        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<MenuItemIngredient>()
            .Requires()
            .AreNotEquals(IngredientId, Guid.Empty, "MenuItemIngredient.IngredientId", "Ingrediente inválido.")
            .AreNotEquals(MenuItemId, Guid.Empty, "MenuItemIngredient.MenuItemId", "Item de menu inválido.");
        
        AddNotifications(contract);
    }
    
    public bool Equals(MenuItemIngredient other)
    {
        return MenuItemId == other.MenuItemId && IngredientId == other.IngredientId;
    }
}