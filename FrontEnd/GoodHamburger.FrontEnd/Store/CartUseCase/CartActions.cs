using GoodHamburger.FrontEnd.Clients.MenuItems;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

public record AddItemAction(MenuItemDto MenuItem);
public record RemoveItemAction(Guid MenuItemId);
public record ClearCartAction();
