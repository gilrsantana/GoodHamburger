using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Catalog.Entities;

public class Ingredient : Entity
{
    public string Name { get; private set; } = null!;
    public string Sku { get; private set; }
    public bool Active { get; private set; } = true;
    public decimal SalePrice { get; private set; }
    public decimal ReferenceCostPrice { get; private set; }
    
    protected Ingredient() { }
    
    private Ingredient(string name, string sku, decimal referenceCostPrice, decimal salePrice)
    {
        Name = name;
        Sku = sku.ToUpper();
        ReferenceCostPrice = referenceCostPrice;
        SalePrice = salePrice;
        
        Validate();
    }
    
    public static Ingredient Create(string name, string sku, decimal referenceCostPrice, decimal salePrice)
    {
        var cost = referenceCostPrice < 0 ? 0 : referenceCostPrice;
        var sale = salePrice < 0 ? 0 : salePrice;
        var ingredient = new Ingredient(name.Trim(), sku.Trim().ToUpper(), cost, sale);
        
        return ingredient;
    }

    public Ingredient UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Ingredient UpdateSku(string sku)
    {
        Sku = sku.Trim().ToUpper();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Ingredient UpdateReferenceCostPrice(decimal referenceCostPrice)
    {
        ReferenceCostPrice = referenceCostPrice < 0 ? 0 : referenceCostPrice;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Ingredient UpdateSalePrice(decimal salePrice)
    {
        SalePrice = salePrice < 0 ? 0 : salePrice;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    public sealed override void Validate()
    {
        AddNotifications(new Contract<Ingredient>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Ingredient.Name", 
                "O nome do ingrediente é obrigatório.")
            .IsNotNullOrEmpty(Sku, "Ingredient.Sku", 
                "O SKU do ingrediente é obrigatório.")
            .IsGreaterOrEqualsThan(ReferenceCostPrice, 0, 
                "Ingredient.ReferenceCostPrice", 
                "O preço de custo não pode ser negativo.")
            .IsGreaterOrEqualsThan(SalePrice, 0, "Ingredient.SalePrice", 
                "O preço de venda não pode ser negativo.")
            .IsGreaterOrEqualsThan(SalePrice, ReferenceCostPrice, "Ingredient.SalePrice", 
                "O preço de venda não pode ser menor que o preço de custo.")
        );
    }
}