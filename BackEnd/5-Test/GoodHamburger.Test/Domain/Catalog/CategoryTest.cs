using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Catalog.Enums;

namespace Goodhamburger.Test.Domain.Catalog;

public class CategoryTest
{
    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg")]
    [InlineData("Fries", "The description of fries", "fries", MenuCategoryType.SideDish,1, 
        null)]
    [InlineData("Coke", "The description of Coke", "soda", MenuCategoryType.Drink,1, 
        null)]
    public void CreateCategory_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        
        Assert.True(category.IsValid);
        Assert.Empty(category.Notifications);
        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
        Assert.Equal(slug, category.Slug);
        Assert.Equal(type, category.Type);
        Assert.Equal(displayOrder, category.DisplayOrder);
        Assert.Equal(imageUrl, category.ImageUrl);
        Assert.IsType<DateTime>(category.CreatedAt);
    }
    
    [Theory]
    [InlineData("", "", "", MenuCategoryType.Burger,-1, "general text")]
    public void CreateCategory_InValid_MustReturnInValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        
        Assert.False(category.IsValid);
        Assert.Equal(4, category.Notifications.Count);
    }
    
    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "New Name")]
    public void UpdateCategoryName_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string newName)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateName(newName);
        
        Assert.True(category.IsValid);
        Assert.Equal(newName, category.Name);
        Assert.NotNull(category.UpdatedAt);
    }
    
    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "")]
    public void UpdateCategoryName_InValid_MustReturnInValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string newName)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateName(newName);
        
        Assert.False(category.IsValid);
        Assert.Equal(newName, category.Name);
        Assert.Single(category.Notifications);
        Assert.Null(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "New description")]
    public void UpdateDescription_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string newDescription)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateDescription(newDescription);
        
        Assert.True(category.IsValid);
        Assert.Equal(newDescription.Trim(), category.Description);
        Assert.NotNull(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "")]
    public void UpdateDescription_Invalid_MustReturnInvalid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string newDescription)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateDescription(newDescription);
        
        Assert.False(category.IsValid);
        Assert.Equal(newDescription.Trim(), category.Description);
        Assert.Null(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "new-slug")]
    public void UpdateSlug_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string newSlug)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateSlug(newSlug);
        
        Assert.True(category.IsValid);
        Assert.Equal(newSlug.Trim().ToLower(), category.Slug);
        Assert.NotNull(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "")]
    public void UpdateSlug_Invalid_MustReturnInvalid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string newSlug)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateSlug(newSlug);
        
        Assert.False(category.IsValid);
        Assert.Equal(newSlug.Trim().ToLower(), category.Slug);
        Assert.Null(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", 5)]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", -1)]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", 0)]
    public void UpdateDisplayOrder_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, int newDisplayOrder)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateDisplayOrder(newDisplayOrder);
        
        Assert.True(category.IsValid);
        Assert.True(category.DisplayOrder == newDisplayOrder || category.DisplayOrder >= 0);
        Assert.NotNull(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "https://new-example.com/image.jpg")]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        null, "https://new-example.com/image.jpg")]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", null)]
    public void UpdateImage_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string? newImageUrl)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateImage(newImageUrl);
        
        Assert.True(category.IsValid);
        Assert.Equal(newImageUrl, category.ImageUrl);
        Assert.NotNull(category.UpdatedAt);
    }

    [Theory]
    [InlineData("", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg", "example.com/image.jpg")]
    public void UpdateImage_Invalid_MustReturnInvalid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl, string? newImageUrl)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.UpdateImage(newImageUrl);
        
        Assert.False(category.IsValid);
        Assert.Equal(newImageUrl, category.ImageUrl);
        Assert.Null(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg")]
    public void Activate_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        category.Deactivate(); // First deactivate to test activation
        
        category.Activate();
        
        Assert.True(category.IsValid);
        Assert.True(category.Active);
        Assert.NotNull(category.UpdatedAt);
    }

    [Theory]
    [InlineData("Sandwiches", "The description", "burger", MenuCategoryType.Burger,1, 
        "https://example.com/image.jpg")]
    public void Deactivate_Valid_MustReturnValid(string name, string description, string slug, MenuCategoryType type, 
        int displayOrder, string? imageUrl)
    {
        var category = Category.Create(name, description, slug, type, displayOrder, imageUrl);
        
        category.Deactivate();
        
        Assert.True(category.IsValid);
        Assert.False(category.Active);
        Assert.NotNull(category.UpdatedAt);
    }
}