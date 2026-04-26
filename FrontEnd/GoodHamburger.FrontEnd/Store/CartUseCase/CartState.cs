using Fluxor;

namespace GoodHamburger.FrontEnd.Store.CartUseCase;

[FeatureState]
public record CartState
{
    public List<CartItem> Items { get; init; } = new();

    public decimal TotalItemsPrice => Items.Sum(i => i.TotalPrice);

    public decimal PotentialDiscountPercentage
    {
        get
        {
            var hasBurger = Items.Any(i => i.MenuItem.CategoryName == "Burger");
            var hasSide = Items.Any(i => i.MenuItem.CategoryName == "SideDish");
            var hasDrink = Items.Any(i => i.MenuItem.CategoryName == "Drink");

            if (hasBurger && hasSide && hasDrink) return 0.20m;
            if (hasBurger && hasDrink) return 0.15m;
            if (hasBurger && hasSide) return 0.10m;
            return 0;
        }
    }

    public decimal DiscountAmount => TotalItemsPrice * PotentialDiscountPercentage;
    public decimal FinalAmount => TotalItemsPrice - DiscountAmount;
}
