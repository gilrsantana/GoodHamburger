using GoodHamburger.FrontEnd.Clients;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

public record CartItem(MenuItemDto MenuItem, int Quantity, List<OrderItemExtraCommand> Extras)
{
    public decimal TotalPrice => (decimal)(MenuItem.Price + Extras.Sum(e => e.Price)) * Quantity;
}
