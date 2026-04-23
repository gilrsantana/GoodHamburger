using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Ordering.Entities;

public class OrderItemDetail: Entity
{
    public Guid OrderItemId { get; private set; }
    public Guid IngredientId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public bool IsRemoved { get; private set; } = false;

    public OrderItemDetail(Guid orderItemId, Guid ingredientId, string name, decimal price)
    {
        OrderItemId = orderItemId;
        IngredientId = ingredientId;
        Name = name;
        Price = price;
        
        AddNotifications(new Contract<OrderItemDetail>()
            .Requires()
            .IsNotNullOrEmpty(Name, "OrderItemDetail.Name", "Name is required.")
            .IsGreaterThan(Price, 0, "OrderItemDetail.Price", "Price must be greater than zero.")
        );
    }
    
    public static OrderItemDetail CreateRemoval(Guid orderItemId, Guid ingredientId, string name)
    {
        var detail = new OrderItemDetail(orderItemId, ingredientId, name, 0);
        detail.IsRemoved = true;
        return detail;
    }

    public override void Validate()
    {
        
    }
}