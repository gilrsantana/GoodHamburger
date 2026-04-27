using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Domain.Ordering.Entities;

namespace GoodHamburger.Application.Services.Ordering.Contracts;

public interface IOrderDiscountService
{
    OrderDiscount? CalculateBestDiscount(Order order);
    
    (decimal? discountAmount, string? discountName) CalculateBestDiscountForCheckout(
        IReadOnlyList<(MenuCategoryType? categoryType, int quantity)> items, 
        decimal subtotal);
}
