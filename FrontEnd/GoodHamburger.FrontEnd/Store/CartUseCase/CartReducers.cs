using Fluxor;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

public static class CartReducers
{
    [ReducerMethod]
    public static CartState OnAddItem(CartState state, AddItemAction action)
    {
        // Category check logic will be handled in the UI/Effect, 
        // but for safety, we could also do it here. 
        // For now, let's just add it.
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
