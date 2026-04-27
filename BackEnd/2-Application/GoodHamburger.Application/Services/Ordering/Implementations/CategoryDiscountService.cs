using GoodHamburger.Application.Services.Ordering.Contracts;
using GoodHamburger.Application.Services.Ordering.Helper;
using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Domain.Ordering.Entities;

namespace GoodHamburger.Application.Services.Ordering.Implementations;

public class CategoryDiscountService : IOrderDiscountService
{
    private readonly Dictionary<HashSet<MenuCategoryType>, decimal> _discountRules;

    public CategoryDiscountService()
    {
        _discountRules = new Dictionary<HashSet<MenuCategoryType>, decimal>(new HashSetEqualityComparer())
        {
            // Burger + SideDish + Drink = 20%
            { [MenuCategoryType.Burger, MenuCategoryType.SideDish, MenuCategoryType.Drink], 0.20m },
            
            // Burger + Drink = 15%
            { [MenuCategoryType.Burger, MenuCategoryType.Drink], 0.15m },
            
            // Burger + SideDish = 10%
            { [MenuCategoryType.Burger, MenuCategoryType.SideDish], 0.10m }
        };
    }

    public OrderDiscount? CalculateBestDiscount(Order order)
    {
        if (order.Items.Count == 0)
            return null;

        var categoriesInOrder = GetCategoriesInOrder(order);
        var bestDiscount = FindBestMatchingDiscount(categoriesInOrder);

        if (bestDiscount == null)
            return null;

        var discountAmount = order.TotalItemsPrice * bestDiscount.Value;
        var discountName = GetDiscountName(categoriesInOrder, bestDiscount.Value);

        return new OrderDiscount(order.Id, discountName, discountAmount);
    }

    private HashSet<MenuCategoryType> GetCategoriesInOrder(Order order)
    {
        var categories = new HashSet<MenuCategoryType>();
        
        foreach (var item in order.Items)
        {
            var category = DetermineCategoryFromItem(item);
            if (category.HasValue)
                categories.Add(category.Value);
        }
        
        return categories;
    }

    private MenuCategoryType? DetermineCategoryFromItem(OrderItem item)
    {
        return item.MenuItem?.Category?.Type;
    }

    private decimal? FindBestMatchingDiscount(HashSet<MenuCategoryType> categoriesInOrder)
    {
        decimal? bestDiscount = null;
        int maxMatchCount = 0;

        foreach (var rule in _discountRules)
        {
            var ruleCategories = rule.Key;
            var discountPercentage = rule.Value;

            if (!ruleCategories.All(categoriesInOrder.Contains)) 
                continue;

            if (ruleCategories.Count <= maxMatchCount &&
                (ruleCategories.Count != maxMatchCount || discountPercentage <= (bestDiscount ?? 0))) 
                continue;
            
            bestDiscount = discountPercentage;
            maxMatchCount = ruleCategories.Count;
        }

        return bestDiscount;
    }

    private string GetDiscountName(HashSet<MenuCategoryType> categories, decimal discountPercentage)
    {
        var categoryNames = categories
            .Select(c => c.ToString())
            .OrderBy(n => n)
            .ToList();

        var categoriesText = string.Join(" + ", categoryNames);
        return $"Combo Discount ({categoriesText}) - {discountPercentage:P0}";
    }

    public (decimal? discountAmount, string? discountName) CalculateBestDiscountForCheckout(
        IReadOnlyList<(MenuCategoryType? categoryType, int quantity)> items, 
        decimal subtotal)
    {
        if (items.Count == 0)
            return (null, null);

        var categoriesInOrder = GetCategoriesInCheckoutItems(items);
        var bestDiscount = FindBestMatchingDiscount(categoriesInOrder);

        if (bestDiscount == null)
            return (null, null);

        var discountAmount = subtotal * bestDiscount.Value;
        var discountName = GetDiscountName(categoriesInOrder, bestDiscount.Value);

        return (discountAmount, discountName);
    }

    private HashSet<MenuCategoryType> GetCategoriesInCheckoutItems(IReadOnlyList<(MenuCategoryType? categoryType, int quantity)> items)
    {
        var categories = new HashSet<MenuCategoryType>();
        
        foreach (var item in items)
        {
            if (item.categoryType.HasValue)
                categories.Add(item.categoryType.Value);
        }
        
        return categories;
    }
}


