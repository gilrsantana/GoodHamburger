using GoodHamburger.FrontEnd.Clients.MenuItems;
using GoodHamburger.FrontEnd.Clients.Orders;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

public record CartItem(MenuItemDto MenuItem, int Quantity, List<OrderItemExtraCommand> Extras)
{
    public decimal TotalPrice => (decimal)(MenuItem.Price ?? 0 + Extras.Sum(e => e.Price ?? 0)) * Quantity;
}
