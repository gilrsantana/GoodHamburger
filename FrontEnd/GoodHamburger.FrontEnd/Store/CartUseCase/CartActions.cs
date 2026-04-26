using GoodHamburger.FrontEnd.Clients;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

public record AddItemAction(MenuItemDto MenuItem);
public record RemoveItemAction(Guid MenuItemId);
public record ClearCartAction();
