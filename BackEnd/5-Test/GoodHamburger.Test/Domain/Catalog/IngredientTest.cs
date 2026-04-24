using GoodHamburger.Domain.Catalog.Entities;

namespace Goodhamburger.Test.Domain.Catalog;

public class IngredientTest
{
    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38)]
    public void CreateIngredient_Valid_MustReturnValid(string name, string code, decimal cost, decimal sale)
    {
        var ingredient = Ingredient.Create(name, code, cost, sale);
        
        Assert.True(ingredient.IsValid);
        Assert.Equal(name, ingredient.Name);
        Assert.Equal(code.ToUpper(), ingredient.Sku);
        Assert.Equal(sale, ingredient.SalePrice);
        Assert.Equal(cost, ingredient.ReferenceCostPrice);
    }

    [Theory]
    [InlineData("", "", 5, 2)]
    public void CreateIngredient_Invalid_MustReturnInvalid(string name, string code, decimal cost, decimal sale)
    {
        var ingredient = Ingredient.Create(name, code, cost, sale);
        
        Assert.False(ingredient.IsValid);
        Assert.NotEmpty(ingredient.Notifications);
        Assert.Equal(3, ingredient.Notifications.Count);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, "New Name")]
    public void UpdateIngredientName_Valid_MustReturnValid(string name, string sku, decimal cost, decimal sale,
        string newName)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        ingredient.UpdateName(newName);
        
        Assert.True(ingredient.IsValid);
        Assert.Equal(newName, ingredient.Name);
        Assert.NotNull(ingredient.UpdatedAt);
    }
    
    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, "")]
    public void UpdateIngredientName_Invalid_MustReturnInvalid(string name, string sku, decimal cost, decimal sale,
        string newName)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        ingredient.UpdateName(newName);
        
        Assert.False(ingredient.IsValid);
        Assert.Equal(newName, ingredient.Name);
        Assert.Single(ingredient.Notifications);
        Assert.Null(ingredient.UpdatedAt);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, "new-sku-123")]
    public void UpdateSku_Valid_MustReturnValid(string name, string sku, decimal cost, decimal sale, string newSku)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        
        ingredient.UpdateSku(newSku);
        
        Assert.True(ingredient.IsValid);
        Assert.Equal(newSku.Trim().ToUpper(), ingredient.Sku);
        Assert.NotNull(ingredient.UpdatedAt);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, "")]
    public void UpdateSku_Invalid_MustReturnInvalid(string name, string sku, decimal cost, decimal sale, string newSku)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        
        ingredient.UpdateSku(newSku);
        
        Assert.False(ingredient.IsValid);
        Assert.Equal(newSku.Trim().ToUpper(), ingredient.Sku);
        Assert.Null(ingredient.UpdatedAt);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, 1.50)]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, 0)]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, -5.00)]
    public void UpdateReferenceCostPrice_Valid_MustReturnValid(string name, string sku, decimal cost, decimal sale, decimal newCost)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        var expectedCost = newCost < 0 ? 0 : newCost;
        
        ingredient.UpdateReferenceCostPrice(newCost);
        
        Assert.True(ingredient.IsValid);
        Assert.Equal(expectedCost, ingredient.ReferenceCostPrice);
        Assert.NotNull(ingredient.UpdatedAt);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "", 0.43, 2.38, 1.50)]
    public void UpdateReferenceCostPrice_Invalid_MustReturnInvalid(string name, string sku, decimal cost, decimal sale, decimal newCost)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        
        ingredient.UpdateReferenceCostPrice(newCost);
        
        Assert.False(ingredient.IsValid);
        Assert.Equal(newCost < 0 ? 0 : newCost, ingredient.ReferenceCostPrice);
        Assert.Null(ingredient.UpdatedAt);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, 3.50)]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0, 0, 0)]
    [InlineData("Pão de Hamburger", "pahbg-12-345", 0.43, 2.38, 2.38)]
    public void UpdateSalePrice_Valid_MustReturnValid(string name, string sku, decimal cost, decimal sale, decimal newSalePrice)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        var expectedSalePrice = newSalePrice < 0 ? 0 : newSalePrice;
        
        ingredient.UpdateSalePrice(newSalePrice);
        
        Assert.True(ingredient.IsValid);
        Assert.Equal(expectedSalePrice, ingredient.SalePrice);
        Assert.NotNull(ingredient.UpdatedAt);
    }

    [Theory]
    [InlineData("Pão de Hamburger", "", 0.43, 2.38, 3.50)]
    public void UpdateSalePrice_Invalid_MustReturnInvalid(string name, string sku, decimal cost, decimal sale, decimal newSalePrice)
    {
        var ingredient = Ingredient.Create(name, sku, cost, sale);
        
        ingredient.UpdateSalePrice(newSalePrice);
        
        Assert.False(ingredient.IsValid);
        Assert.Equal(newSalePrice < 0 ? 0 : newSalePrice, ingredient.SalePrice);
        Assert.Null(ingredient.UpdatedAt);
    }
}