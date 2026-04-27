using Fluxor;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

public static class CartReducers
{
    [ReducerMethod]
    public static CartState OnAddItem(CartState state, AddItemAction action)
    {
        var existingItem = state.Items.FirstOrDefault(i => i.MenuItem.Id == action.MenuItem.Id);
        
        if (existingItem != null)
        {
            // Increment quantity if item already exists
            return state with
            {
                Items = state.Items.Select(i => 
                    i.MenuItem.Id == action.MenuItem.Id 
                        ? i with { Quantity = i.Quantity + 1 } 
                        : i).ToList()
            };
        }
        
        // Add new item if it doesn't exist
        return state with
        {
            Items = new List<CartItem>(state.Items) { new CartItem(action.MenuItem, 1, new()) }
        };
    }

    [ReducerMethod]
    public static CartState OnRemoveItem(CartState state, RemoveItemAction action)
    {
        return state with
        {
            Items = state.Items.Where(i => i.MenuItem.Id != action.MenuItemId).ToList()
        };
    }

    [ReducerMethod]
    public static CartState OnClearCart(CartState state, ClearCartAction action)
    {
        return state with { Items = new() };
    }
}
