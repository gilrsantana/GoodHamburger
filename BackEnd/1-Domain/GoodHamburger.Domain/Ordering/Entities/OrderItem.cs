using Flunt.Validations;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Ordering.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; } = Guid.Empty;
    
    // Referência ao Catálogo
    public Guid MenuItemId { get; private set; }
    public MenuItem? MenuItem { get; set; }
    public string ProductName { get; private set; } = null!;
    public string Sku { get; private set; }  = null!;
    
    // Valores financeiros imutáveis para este pedido
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public string? Note { get; private set; }
    
    private readonly List<OrderItemDetail> _details = new();
    public IReadOnlyCollection<OrderItemDetail> Details => _details;
    
    protected OrderItem() { }
    
    public OrderItem(
        Guid menuItemId, 
        string productName, 
        string sku,
        int quantity, 
        decimal unitPrice, 
        string? note = null)
    {
        MenuItemId = menuItemId;
        ProductName = productName;
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Note = note;
        
        AddNotifications(new Contract<OrderItem>()
            .Requires()
            .AreNotEquals(MenuItemId, Guid.Empty, "OrderItem.MenuItemId", "Invalid product reference")
            .IsNotNullOrEmpty(ProductName, "OrderItem.ProductName", "Product name is required.")
            .IsGreaterThan(Quantity, 0, "OrderItem.Quantity", "Quantity must greater than zero.")
            .IsGreaterThan(UnitPrice, 0, "OrderItem.UnitPrice", "Unit price must greater than zero.")
        );
    }
    
    public void AddExtra(Guid ingredientId, string name, decimal price)
    {
        // Aqui instanciamos o detalhe que você viu anteriormente
        _details.Add(new OrderItemDetail(Id, ingredientId, name, price));
    }

    public decimal TotalItemPrice 
        => (UnitPrice + _details.Sum(d => d.Price)) * Quantity;

    public override void Validate()
    {
        
    }
}