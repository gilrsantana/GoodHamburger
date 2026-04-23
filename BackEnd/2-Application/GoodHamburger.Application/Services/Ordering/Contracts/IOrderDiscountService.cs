using GoodHamburger.Domain.Ordering.Entities;

namespace GoodHamburger.Application.Services.Ordering.Contracts;

public interface IOrderDiscountService
{
    OrderDiscount? CalculateBestDiscount(Order order);
}
