using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Catalog.Enums;

namespace Goodhamburger.Test.Domain.Catalog;

public class MenuItemTest
{
    [Theory]
    [InlineData("Sandwiches", "The description", "burger", 
        "brg-15-10", 10.99, 100, "https://example.com/image.jpg")]
    [InlineData("Fries", "The description", "fries", 
        "frs-22-00", 5.99, 50, null)]
    public void CreateMenuItem_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, int? calories, string? imageUrl)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId, calories, imageUrl);
        
        Assert.True(menuItem.IsValid);
        Assert.Empty(menuItem.Notifications);
        Assert.Equal(name, menuItem.Name);
        Assert.Equal(description, menuItem.Description);
        Assert.Equal(slug, menuItem.Slug);
        Assert.Equal(price, menuItem.Price);
        Assert.Equal(categoryId, menuItem.CategoryId);
        Assert.Equal(calories, menuItem.Calories);
        Assert.Equal(imageUrl, menuItem.ImageUrl);
    }
    
    [Theory]
    [InlineData("", "", "", "", -10, -5, "example.com/image.jpg")]
    public void CreateMenuItem_InValid_MustReturnInValid(string name, string description, string slug, 
        string sku, decimal price, int? calories, string? imageUrl)
    {
        Guid categoryId = Guid.Empty;
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId, calories, imageUrl);
        
        Assert.False(menuItem.IsValid);
        Assert.Equal(7, menuItem.Notifications.Count);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "New Burger Name")]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "   New Burger Name   ")]
    public void UpdateName_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, string newName)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateName(newName);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(newName.Trim(), menuItem.Name);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "")]
    public void UpdateName_Invalid_MustReturnInvalid(string name, string description, string slug, 
        string sku, decimal price, string newName)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateName(newName);
        
        Assert.False(menuItem.IsValid);
        Assert.Single(menuItem.Notifications);
        Assert.Equal(newName, menuItem.Name);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "New Description")]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "   New Description   ")]
    public void UpdateDescription_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, string newDescription)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateDescription(newDescription);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(newDescription.Trim(), menuItem.Description);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "")]
    public void UpdateDescription_Invalid_MustReturnInvalid(string name, string description, string slug, 
        string sku, decimal price, string newDescription)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateDescription(newDescription);
        
        Assert.False(menuItem.IsValid);
        Assert.Single(menuItem.Notifications);
        Assert.Equal(newDescription, menuItem.Description);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "new-slug")]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "   new-slug   ")]
    public void UpdateSlug_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, string newSlug)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateSlug(newSlug);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(newSlug.Trim(), menuItem.Slug);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "")]
    public void UpdateSlug_Invalid_MustReturnInvalid(string name, string description, string slug, 
        string sku, decimal price, string newSlug)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateSlug(newSlug);
        
        Assert.False(menuItem.IsValid);
        Assert.Single(menuItem.Notifications);
        Assert.Equal(newSlug, menuItem.Slug);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "new-sku")]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "   new-sku   ")]
    public void UpdateSku_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, string newSku)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateSku(newSku);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(newSku.Trim().ToUpper(), menuItem.Sku);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "")]
    public void UpdateSku_Invalid_MustReturnInvalid(string name, string description, string slug, 
        string sku, decimal price, string newSku)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateSku(newSku);
        
        Assert.False(menuItem.IsValid);
        Assert.Single(menuItem.Notifications);
        Assert.Equal(newSku.Trim().ToUpper(), menuItem.Sku);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, 15.99)]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, 0.1)]
    public void UpdatePrice_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, decimal newPrice)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        var expectedPrice = newPrice < 0 ? 0 : newPrice;
        
        menuItem.UpdatePrice(newPrice);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(expectedPrice, menuItem.Price);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, -5.00)]
    public void UpdatePrice_Invalid_MustReturnZeroInvalid(string name, string description, string slug, 
        string sku, decimal price, decimal newPrice)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        var expectedPrice = newPrice < 0 ? 0 : newPrice;
        
        menuItem.UpdatePrice(newPrice);
        
        Assert.False(menuItem.IsValid);
        Assert.Equal(expectedPrice, menuItem.Price);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "https://example.com/image.jpg")]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "")]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, null)]
    public void UpdateImage_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, string? newImageUrl)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateImage(newImageUrl);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(newImageUrl, menuItem.ImageUrl);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, "invalid-url")]
    public void UpdateImage_Invalid_MustReturnInvalid(string name, string description, string slug, 
        string sku, decimal price, string newImageUrl)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateImage(newImageUrl);
        
        Assert.False(menuItem.IsValid);
        Assert.Equal(newImageUrl, menuItem.ImageUrl);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, 250)]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, 0)]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, -50)]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99, null)]
    public void UpdateCalories_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price, int? newCalories)
    {
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        var expectedCalories = newCalories is < 0 ? 0 : newCalories;
        
        menuItem.UpdateCalories(newCalories);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(expectedCalories, menuItem.Calories);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99)]
    public void UpdateCategoryId_Valid_MustReturnValid(string name, string description, string slug, 
        string sku, decimal price)
    {
        Guid categoryId = Guid.CreateVersion7();
        Guid newCategoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateCategoryId(newCategoryId);
        
        Assert.True(menuItem.IsValid);
        Assert.Equal(newCategoryId, menuItem.CategoryId);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Theory]
    [InlineData("Original Burger", "Description", "slug", "SKU", 10.99)]
    public void UpdateCategoryId_Invalid_MustReturnInvalid(string name, string description, string slug, 
        string sku, decimal price)
    {
        Guid emptyCategoryId = Guid.Empty;
        Guid categoryId = Guid.CreateVersion7();
        var menuItem = MenuItem.Create(name, description, slug, sku, price, categoryId);
        
        menuItem.UpdateCategoryId(emptyCategoryId);
        
        Assert.False(menuItem.IsValid);
        Assert.Equal(emptyCategoryId, menuItem.CategoryId);
        Assert.Null(menuItem.UpdatedAt);
    }

    [Fact]
    public void UpdateCategory_Valid_MustReturnValid()
    {
        // Arrange
        var menuItem = MenuItem.Create("Original Burger", "Description", "slug", "SKU", 10.99m, Guid.CreateVersion7());
        var category = Category.Create("Burgers", "Burger category", "burgers", MenuCategoryType.Burger, 1);
        
        // Act
        menuItem.UpdateCategory(category);
        
        // Assert
        Assert.True(menuItem.IsValid);
        Assert.Equal(category, menuItem.Category);
        Assert.NotNull(menuItem.UpdatedAt);
    }

    [Fact]
    public void UpdateCategory_Invalid_MustReturnInvalid()
    {
        // Arrange
        var menuItem = MenuItem.Create("Original Burger", "Description", "slug", "SKU", 10.99m, Guid.CreateVersion7());
        var invalidCategory = Category.Create("", "", "", MenuCategoryType.Burger, 1);
        
        // Act
        menuItem.UpdateCategory(invalidCategory);
        
        // Assert
        Assert.False(menuItem.IsValid);
        Assert.Equal(invalidCategory, menuItem.Category);
        Assert.Null(menuItem.UpdatedAt);
    }

}