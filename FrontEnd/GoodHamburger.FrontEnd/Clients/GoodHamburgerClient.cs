using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace GoodHamburger.FrontEnd.Clients;

public interface IGoodHamburgerClient
{
    // Auth
    Task<LoginResponse> LoginAsync(LoginCommand command);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenCommand command);
    Task<LogoutResponse> LogoutAsync();

    // Categories
    Task<GetAllCategoriesResponse> GetAllCategoriesAsync(int page = 1, int pageSize = 10);
    Task<GetCategoryByIdResponse> GetCategoryByIdAsync(Guid id);
    Task<GetCategoryBySlugResponse> GetCategoryBySlugAsync(string slug);
    Task<GetCategoriesByTypeResponse> GetCategoriesByTypeAsync(MenuCategoryType type, int page = 1, int pageSize = 10);
    Task<GetActiveCategoriesResponse> GetActiveCategoriesAsync(int page = 1, int pageSize = 10);
    Task<CreateCategoryResponse> CreateCategoryAsync(CreateCategoryCommand command);
    Task<UpdateCategoryResponse> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command);
    Task<DeleteCategoryResponse> DeleteCategoryAsync(Guid id);
    Task<ActivateCategoryResponse> ActivateCategoryAsync(Guid id);
    Task<DeactivateCategoryResponse> DeactivateCategoryAsync(Guid id);

    // Ingredients
    Task<GetAllIngredientsResponse> GetAllIngredientsAsync(int page = 1, int pageSize = 10);
    Task<GetIngredientByIdResponse> GetIngredientByIdAsync(Guid id);
    Task<GetActiveIngredientsResponse> GetActiveIngredientsAsync(int page = 1, int pageSize = 10);
    Task<SearchIngredientsResponse> SearchIngredientsAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<GetIngredientsByPriceRangeResponse> GetIngredientsByPriceRangeAsync(double minPrice, double maxPrice, int page = 1, int pageSize = 10);
    Task<CreateIngredientResponse> CreateIngredientAsync(CreateIngredientCommand command);
    Task<UpdateIngredientResponse> UpdateIngredientAsync(Guid id, UpdateIngredientCommand command);
    Task<DeleteIngredientResponse> DeleteIngredientAsync(Guid id);
    Task<ActivateIngredientResponse> ActivateIngredientAsync(Guid id);
    Task<DeactivateIngredientResponse> DeactivateIngredientAsync(Guid id);

    // MenuItems
    Task<GetAllMenuItemsResponse> GetAllMenuItemsAsync(int page = 1, int pageSize = 10);
    Task<GetMenuItemByIdResponse> GetMenuItemByIdAsync(Guid id);
    Task<GetMenuItemBySkuResponse> GetMenuItemBySkuAsync(string sku);
    Task<GetMenuItemsByCategoryResponse> GetMenuItemsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10);
    Task<GetActiveMenuItemsResponse> GetActiveMenuItemsAsync(int page = 1, int pageSize = 10);
    Task<GetAvailableMenuItemsResponse> GetAvailableMenuItemsAsync(int page = 1, int pageSize = 10);
    Task<SearchMenuItemsResponse> SearchMenuItemsAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<GetMenuItemsByPriceRangeResponse> GetMenuItemsByPriceRangeAsync(double minPrice, double maxPrice, int page = 1, int pageSize = 10);
    Task<CreateMenuItemResponse> CreateMenuItemAsync(CreateMenuItemCommand command);
    Task<UpdateMenuItemResponse> UpdateMenuItemAsync(Guid id, UpdateMenuItemCommand command);
    Task<DeleteMenuItemResponse> DeleteMenuItemAsync(Guid id);
    Task<ActivateMenuItemResponse> ActivateMenuItemAsync(Guid id);
    Task<DeactivateMenuItemResponse> DeactivateMenuItemAsync(Guid id);
    Task<SetMenuItemAvailabilityResponse> SetMenuItemAvailabilityAsync(Guid id, bool isAvailable);
    Task<AddIngredientToMenuItemResponse> AddIngredientToMenuItemAsync(Guid menuItemId, Guid ingredientId, bool isRemovable = true);
    Task<RemoveIngredientFromMenuItemResponse> RemoveIngredientFromMenuItemAsync(Guid menuItemId, Guid ingredientId);

    // Orders
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderCommand command);
    Task<GetAllOrdersResponse> GetAllOrdersAsync(int page = 1, int pageSize = 10);
    Task<GetOrderByIdResponse> GetOrderByIdAsync(Guid id);
    Task<GetOrdersByCustomerResponse> GetOrdersByCustomerAsync(Guid customerId, int page = 1, int pageSize = 10);
    Task<GetOrdersByStatusResponse> GetOrdersByStatusAsync(OrderStatus status, int page = 1, int pageSize = 10);
    Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusCommand command);
    Task<CancelOrderResponse> CancelOrderAsync(Guid id, string reason);
    Task<CheckoutCalculationResponse> CalculateCheckoutAsync(CheckoutCalculationQuery query);
    Task<ViaCepAddressDto?> GetAddressByZipCodeAsync(string zipCode);

    // Coupons
    Task<CreateCouponResponse> CreateCouponAsync(CreateCouponCommand command);
    Task<GetAllCouponsResponse> GetAllCouponsAsync(int page = 1, int pageSize = 10);
    Task<GetCouponByIdResponse> GetCouponByIdAsync(Guid id);
    Task<GetCouponByCodeResponse> GetCouponByCodeAsync(string code);
    Task<GetActiveCouponsResponse> GetActiveCouponsAsync(int page = 1, int pageSize = 10);
    Task<UpdateCouponResponse> UpdateCouponAsync(Guid id, UpdateCouponCommand command);
    Task<CancelCouponResponse> CancelCouponAsync(Guid id);

    // Identity
    Task<GetAllUsersResponse> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<GetUserResponse> GetUserByIdAsync(string id);
    Task<CreateUserResponse> CreateUserAsync(object command);
    Task<UpdateUserResponse> UpdateUserAsync(string id, object command);
    Task<DeleteUserResponse> DeleteUserAsync(string id);

    // Customer Profiles
    Task<GetCustomerProfileResponse> GetCustomerProfileAsync(Guid identityId);
    Task<UpdateCustomerProfileResponse> UpdateCustomerProfileAsync(Guid id, UpdateCustomerProfileCommand command);
    Task<UpdateCustomerDocumentResponse> UpdateCustomerDocumentAsync(Guid id, UpdateCustomerDocumentCommand command);
    Task<UpdateCustomerAddressResponse> UpdateCustomerAddressAsync(Guid id, UpdateCustomerAddressCommand command);
    Task<UpdateCustomerBirthDateResponse> UpdateCustomerBirthDateAsync(Guid id, UpdateCustomerBirthDateCommand command);
    Task<GetAllRolesResponse> GetAllRolesAsync();
    Task<CreateRoleResponse> CreateRoleAsync(object command);
    Task<AssignRoleToUserResponse> AssignRoleToUserAsync(object command);
    Task<RemoveRoleFromUserResponse> RemoveRoleFromUserAsync(object command);
}

public class GoodHamburgerClient : IGoodHamburgerClient
{
    private readonly HttpClient _httpClient;

    public GoodHamburgerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task<T> PostAsync<T>(string url, object? body)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response body is null");
    }

    private async Task<T> GetAsync<T>(string url)
    {
        return await _httpClient.GetFromJsonAsync<T>(url) ?? throw new InvalidOperationException("Response body is null");
    }

    private async Task<T> DeleteAsync<T>(string url)
    {
        var response = await _httpClient.DeleteAsync(url);
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response body is null");
    }

    // Auth
    public Task<LoginResponse> LoginAsync(LoginCommand command) => PostAsync<LoginResponse>("api/Auth/login", command);
    public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenCommand command) => PostAsync<RefreshTokenResponse>("api/Auth/refresh-token", command);
    public async Task<LogoutResponse> LogoutAsync()
    {
        var response = await _httpClient.PostAsync("api/Auth/logout", null);
        return await response.Content.ReadFromJsonAsync<LogoutResponse>() ?? throw new InvalidOperationException();
    }

    // Categories
    public Task<GetAllCategoriesResponse> GetAllCategoriesAsync(int page = 1, int pageSize = 10) => GetAsync<GetAllCategoriesResponse>($"api/Categories?page={page}&pageSize={pageSize}");
    public Task<GetCategoryByIdResponse> GetCategoryByIdAsync(Guid id) => GetAsync<GetCategoryByIdResponse>($"api/Categories/{id}");
    public Task<GetCategoryBySlugResponse> GetCategoryBySlugAsync(string slug) => GetAsync<GetCategoryBySlugResponse>($"api/Categories/slug/{slug}");
    public Task<GetCategoriesByTypeResponse> GetCategoriesByTypeAsync(MenuCategoryType type, int page = 1, int pageSize = 10) => GetAsync<GetCategoriesByTypeResponse>($"api/Categories/type/{type}?page={page}&pageSize={pageSize}");
    public Task<GetActiveCategoriesResponse> GetActiveCategoriesAsync(int page = 1, int pageSize = 10) => GetAsync<GetActiveCategoriesResponse>($"api/Categories/active?page={page}&pageSize={pageSize}");
    public Task<CreateCategoryResponse> CreateCategoryAsync(CreateCategoryCommand command) => PostAsync<CreateCategoryResponse>("api/Categories", command);
    public async Task<UpdateCategoryResponse> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Categories/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateCategoryResponse>() ?? throw new InvalidOperationException();
    }
    public async Task<DeleteCategoryResponse> DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Categories/{id}");
        return await response.Content.ReadFromJsonAsync<DeleteCategoryResponse>() ?? throw new InvalidOperationException();
    }
    public Task<ActivateCategoryResponse> ActivateCategoryAsync(Guid id) => PostAsync<ActivateCategoryResponse>($"api/Categories/{id}/activate", null);
    public Task<DeactivateCategoryResponse> DeactivateCategoryAsync(Guid id) => PostAsync<DeactivateCategoryResponse>($"api/Categories/{id}/deactivate", null);

    // Ingredients
    public Task<GetAllIngredientsResponse> GetAllIngredientsAsync(int page = 1, int pageSize = 10) => GetAsync<GetAllIngredientsResponse>($"api/Ingredients?page={page}&pageSize={pageSize}");
    public async Task<GetIngredientByIdResponse> GetIngredientByIdAsync(Guid id) => await GetAsync<GetIngredientByIdResponse>($"api/Ingredients/{id}");
    public Task<GetActiveIngredientsResponse> GetActiveIngredientsAsync(int page = 1, int pageSize = 10) => GetAsync<GetActiveIngredientsResponse>($"api/Ingredients/active?page={page}&pageSize={pageSize}");
    public Task<SearchIngredientsResponse> SearchIngredientsAsync(string searchTerm, int page = 1, int pageSize = 10) => GetAsync<SearchIngredientsResponse>($"api/Ingredients/search?searchTerm={searchTerm}&page={page}&pageSize={pageSize}");
    public Task<GetIngredientsByPriceRangeResponse> GetIngredientsByPriceRangeAsync(double minPrice, double maxPrice, int page = 1, int pageSize = 10) => GetAsync<GetIngredientsByPriceRangeResponse>($"api/Ingredients/price-range?minPrice={minPrice}&maxPrice={maxPrice}&page={page}&pageSize={pageSize}");
    public Task<CreateIngredientResponse> CreateIngredientAsync(CreateIngredientCommand command) => PostAsync<CreateIngredientResponse>("api/Ingredients", command);
    public async Task<UpdateIngredientResponse> UpdateIngredientAsync(Guid id, UpdateIngredientCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Ingredients/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateIngredientResponse>() ?? throw new InvalidOperationException();
    }
    public async Task<DeleteIngredientResponse> DeleteIngredientAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Ingredients/{id}");
        return await response.Content.ReadFromJsonAsync<DeleteIngredientResponse>() ?? throw new InvalidOperationException();
    }
    public Task<ActivateIngredientResponse> ActivateIngredientAsync(Guid id) => PostAsync<ActivateIngredientResponse>($"api/Ingredients/{id}/activate", null);
    public Task<DeactivateIngredientResponse> DeactivateIngredientAsync(Guid id) => PostAsync<DeactivateIngredientResponse>($"api/Ingredients/{id}/deactivate", null);

    // MenuItems
    public Task<GetAllMenuItemsResponse> GetAllMenuItemsAsync(int page = 1, int pageSize = 10) => GetAsync<GetAllMenuItemsResponse>($"api/MenuItems?page={page}&pageSize={pageSize}");
    public Task<GetMenuItemByIdResponse> GetMenuItemByIdAsync(Guid id) => GetAsync<GetMenuItemByIdResponse>($"api/MenuItems/{id}");
    public Task<GetMenuItemBySkuResponse> GetMenuItemBySkuAsync(string sku) => GetAsync<GetMenuItemBySkuResponse>($"api/MenuItems/sku/{sku}");
    public Task<GetMenuItemsByCategoryResponse> GetMenuItemsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10) => GetAsync<GetMenuItemsByCategoryResponse>($"api/MenuItems/category/{categoryId}?page={page}&pageSize={pageSize}");
    public Task<GetActiveMenuItemsResponse> GetActiveMenuItemsAsync(int page = 1, int pageSize = 10) => GetAsync<GetActiveMenuItemsResponse>($"api/MenuItems/active?page={page}&pageSize={pageSize}");
    public Task<GetAvailableMenuItemsResponse> GetAvailableMenuItemsAsync(int page = 1, int pageSize = 10) => GetAsync<GetAvailableMenuItemsResponse>($"api/MenuItems/available?page={page}&pageSize={pageSize}");
    public Task<SearchMenuItemsResponse> SearchMenuItemsAsync(string searchTerm, int page = 1, int pageSize = 10) => GetAsync<SearchMenuItemsResponse>($"api/MenuItems/search?searchTerm={searchTerm}&page={page}&pageSize={pageSize}");
    public Task<GetMenuItemsByPriceRangeResponse> GetMenuItemsByPriceRangeAsync(double minPrice, double maxPrice, int page = 1, int pageSize = 10) => GetAsync<GetMenuItemsByPriceRangeResponse>($"api/MenuItems/price-range?minPrice={minPrice}&maxPrice={maxPrice}&page={page}&pageSize={pageSize}");
    public Task<CreateMenuItemResponse> CreateMenuItemAsync(CreateMenuItemCommand command) => PostAsync<CreateMenuItemResponse>("api/MenuItems", command);
    public async Task<UpdateMenuItemResponse> UpdateMenuItemAsync(Guid id, UpdateMenuItemCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/MenuItems/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateMenuItemResponse>() ?? throw new InvalidOperationException();
    }
    public async Task<DeleteMenuItemResponse> DeleteMenuItemAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/MenuItems/{id}");
        return await response.Content.ReadFromJsonAsync<DeleteMenuItemResponse>() ?? throw new InvalidOperationException();
    }
    public Task<ActivateMenuItemResponse> ActivateMenuItemAsync(Guid id) => PostAsync<ActivateMenuItemResponse>($"api/MenuItems/{id}/activate", null);
    public Task<DeactivateMenuItemResponse> DeactivateMenuItemAsync(Guid id) => PostAsync<DeactivateMenuItemResponse>($"api/MenuItems/{id}/deactivate", null);
    public Task<SetMenuItemAvailabilityResponse> SetMenuItemAvailabilityAsync(Guid id, bool isAvailable) => PostAsync<SetMenuItemAvailabilityResponse>($"api/MenuItems/{id}/availability?isAvailable={isAvailable}", null);
    public Task<AddIngredientToMenuItemResponse> AddIngredientToMenuItemAsync(Guid menuItemId, Guid ingredientId, bool isRemovable = true) => PostAsync<AddIngredientToMenuItemResponse>($"api/MenuItems/{menuItemId}/ingredients/{ingredientId}?isRemovable={isRemovable}", null);
    public Task<RemoveIngredientFromMenuItemResponse> RemoveIngredientFromMenuItemAsync(Guid menuItemId, Guid ingredientId) => DeleteAsync<RemoveIngredientFromMenuItemResponse>($"api/MenuItems/{menuItemId}/ingredients/{ingredientId}");

    // Orders
    public Task<CreateOrderResponse> CreateOrderAsync(CreateOrderCommand command) => PostAsync<CreateOrderResponse>("api/Orders", command);
    public Task<GetAllOrdersResponse> GetAllOrdersAsync(int page = 1, int pageSize = 10) => GetAsync<GetAllOrdersResponse>($"api/Orders?page={page}&pageSize={pageSize}");
    public Task<GetOrderByIdResponse> GetOrderByIdAsync(Guid id) => GetAsync<GetOrderByIdResponse>($"api/Orders/{id}");
    public Task<GetOrdersByCustomerResponse> GetOrdersByCustomerAsync(Guid customerId, int page = 1, int pageSize = 10) => GetAsync<GetOrdersByCustomerResponse>($"api/Orders/customer/{customerId}?page={page}&pageSize={pageSize}");
    public Task<GetOrdersByStatusResponse> GetOrdersByStatusAsync(OrderStatus status, int page = 1, int pageSize = 10) => GetAsync<GetOrdersByStatusResponse>($"api/Orders/status/{status}?page={page}&pageSize={pageSize}");
    public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusCommand command)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/Orders/{id}/status", command);
        return await response.Content.ReadFromJsonAsync<UpdateOrderStatusResponse>() ?? throw new InvalidOperationException();
    }
    public Task<CancelOrderResponse> CancelOrderAsync(Guid id, string reason) => PostAsync<CancelOrderResponse>($"api/Orders/{id}/cancel", reason);
    public Task<CheckoutCalculationResponse> CalculateCheckoutAsync(CheckoutCalculationQuery query) => PostAsync<CheckoutCalculationResponse>("api/Orders/checkout/calculate", query);
    public async Task<ViaCepAddressDto?> GetAddressByZipCodeAsync(string zipCode)
    {
        try
        {
            return await GetAsync<ViaCepAddressDto>($"api/Address/via-cep/{zipCode}");
        }
        catch
        {
            return null;
        }
    }

    // Coupons
    public Task<CreateCouponResponse> CreateCouponAsync(CreateCouponCommand command) => PostAsync<CreateCouponResponse>("api/Coupons", command);
    public Task<GetAllCouponsResponse> GetAllCouponsAsync(int page = 1, int pageSize = 10) => GetAsync<GetAllCouponsResponse>($"api/Coupons?page={page}&pageSize={pageSize}");
    public Task<GetCouponByIdResponse> GetCouponByIdAsync(Guid id) => GetAsync<GetCouponByIdResponse>($"api/Coupons/{id}");
    public Task<GetCouponByCodeResponse> GetCouponByCodeAsync(string code) => GetAsync<GetCouponByCodeResponse>($"api/Coupons/code/{code}");
    public Task<GetActiveCouponsResponse> GetActiveCouponsAsync(int page = 1, int pageSize = 10) => GetAsync<GetActiveCouponsResponse>($"api/Coupons/active?page={page}&pageSize={pageSize}");
    public async Task<UpdateCouponResponse> UpdateCouponAsync(Guid id, UpdateCouponCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Coupons/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateCouponResponse>() ?? throw new InvalidOperationException();
    }
    public Task<CancelCouponResponse> CancelCouponAsync(Guid id) => PostAsync<CancelCouponResponse>($"api/Coupons/{id}/cancel", null);

    // Identity
    public Task<GetAllUsersResponse> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null) => GetAsync<GetAllUsersResponse>($"api/Users?page={page}&pageSize={pageSize}&search={search}");
    public Task<GetUserResponse> GetUserByIdAsync(string id) => GetAsync<GetUserResponse>($"api/Users/{id}");
    public Task<CreateUserResponse> CreateUserAsync(object command) => PostAsync<CreateUserResponse>("api/Users", command);
    public Task<UpdateUserResponse> UpdateUserAsync(string id, object command) => PostAsync<UpdateUserResponse>($"api/Users/{id}", command);
    public Task<DeleteUserResponse> DeleteUserAsync(string id) => PostAsync<DeleteUserResponse>($"api/Users/{id}", null);
    public Task<GetAllRolesResponse> GetAllRolesAsync() => GetAsync<GetAllRolesResponse>("api/Roles");
    public Task<CreateRoleResponse> CreateRoleAsync(object command) => PostAsync<CreateRoleResponse>("api/Roles", command);
    public Task<AssignRoleToUserResponse> AssignRoleToUserAsync(object command) => PostAsync<AssignRoleToUserResponse>("api/Roles/assign", command);
    public Task<RemoveRoleFromUserResponse> RemoveRoleFromUserAsync(object command) => PostAsync<RemoveRoleFromUserResponse>("api/Roles/remove", command);

    // Customer Profiles
    public Task<GetCustomerProfileResponse> GetCustomerProfileAsync(Guid identityId) => GetAsync<GetCustomerProfileResponse>($"api/CustomerProfiles/by-identity/{identityId}");
    public async Task<UpdateCustomerProfileResponse> UpdateCustomerProfileAsync(Guid id, UpdateCustomerProfileCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/CustomerProfiles/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateCustomerProfileResponse>() ?? throw new InvalidOperationException();
    }
    public async Task<UpdateCustomerDocumentResponse> UpdateCustomerDocumentAsync(Guid id, UpdateCustomerDocumentCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/CustomerProfiles/{id}/document", command);
        return await response.Content.ReadFromJsonAsync<UpdateCustomerDocumentResponse>() ?? throw new InvalidOperationException();
    }
    public async Task<UpdateCustomerAddressResponse> UpdateCustomerAddressAsync(Guid id, UpdateCustomerAddressCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/CustomerProfiles/{id}/address", command);
        return await response.Content.ReadFromJsonAsync<UpdateCustomerAddressResponse>() ?? throw new InvalidOperationException();
    }
    public async Task<UpdateCustomerBirthDateResponse> UpdateCustomerBirthDateAsync(Guid id, UpdateCustomerBirthDateCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/CustomerProfiles/{id}/birth-date", command);
        return await response.Content.ReadFromJsonAsync<UpdateCustomerBirthDateResponse>() ?? throw new InvalidOperationException();
    }
}

// --- DTOs ---

public class LoginCommand 
{ 
    public LoginCommand() { }
    public LoginCommand(string email, string password) { Email = email; Password = password; }
    public string Email { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty; 
}
public record LoginResponse(Guid UserId, string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, bool Succeeded, IEnumerable<string>? Errors);
public record RefreshTokenCommand(string AccessToken, string RefreshToken);
public record RefreshTokenResponse(Guid UserId, string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, bool Succeeded, IEnumerable<string>? Errors);
public record LogoutResponse(bool Succeeded, IEnumerable<string>? Errors);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MenuCategoryType { Burger, SideDish, Drink }

public class CategoryDto 
{ 
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public MenuCategoryType Type { get; set; }
    public bool Active { get; set; }
    public int DisplayOrder { get; set; }
    public string? ImageUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public record GetAllCategoriesResponse(IEnumerable<CategoryDto> Items, int TotalCount, int Page, int PageSize);
public record GetCategoryByIdResponse(CategoryDto? Category);
public record GetCategoryBySlugResponse(CategoryDto? Category);
public record GetCategoriesByTypeResponse(IEnumerable<CategoryDto> Items, int TotalCount, int Page, int PageSize);
public record GetActiveCategoriesResponse(IEnumerable<CategoryDto> Items, int TotalCount, int Page, int PageSize);
public class CreateCategoryCommand { public string Name { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public MenuCategoryType Type { get; set; } public int DisplayOrder { get; set; } public string? ImageUrl { get; set; } }
public record CreateCategoryResponse(Guid Id, string Name, string Slug, bool Succeeded, IEnumerable<string>? Errors);
public class UpdateCategoryCommand { public Guid CategoryId { get; set; } public string? Name { get; set; } public string? Description { get; set; } public string? Slug { get; set; } public int? DisplayOrder { get; set; } public string? ImageUrl { get; set; } }
public record UpdateCategoryResponse(Guid Id, string Name, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteCategoryResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
public record ActivateCategoryResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record DeactivateCategoryResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public bool Active { get; set; }
    public double SalePrice { get; set; }
    public double ReferenceCostPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public record GetAllIngredientsResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public record GetIngredientByIdResponse(IngredientDto? Ingredient);
public record GetActiveIngredientsResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public record SearchIngredientsResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public record GetIngredientsByPriceRangeResponse(IEnumerable<IngredientDto> Items, int TotalCount, int Page, int PageSize);
public class CreateIngredientCommand { public string Name { get; set; } = string.Empty; public string Sku { get; set; } = string.Empty; public double ReferenceCostPrice { get; set; } public double SalePrice { get; set; } }
public record CreateIngredientResponse(Guid Id, string Name, string Sku, bool Succeeded, IEnumerable<string>? Errors);
public class UpdateIngredientCommand { public Guid IngredientId { get; set; } public string? Name { get; set; } public string? Sku { get; set; } public double? ReferenceCostPrice { get; set; } public double? SalePrice { get; set; } }
public record UpdateIngredientResponse(Guid Id, string Name, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteIngredientResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
public record ActivateIngredientResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record DeactivateIngredientResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);

public record MenuItemIngredientDto(Guid IngredientId, string IngredientName, string IngredientSku, bool IsRemovable);

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public double? Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool Active { get; set; }
    public bool IsAvailable { get; set; }
    public int? Calories { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public IEnumerable<MenuItemIngredientDto> Ingredients { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public record GetAllMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetMenuItemByIdResponse(MenuItemDto? MenuItem);
public record GetMenuItemBySkuResponse(MenuItemDto? MenuItem);
public record GetMenuItemsByCategoryResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetActiveMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetAvailableMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record SearchMenuItemsResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public record GetMenuItemsByPriceRangeResponse(IEnumerable<MenuItemDto> Items, int TotalCount, int Page, int PageSize);
public class CreateMenuItemCommand { public string Name { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public string Sku { get; set; } = string.Empty; public double Price { get; set; } public Guid CategoryId { get; set; } public int? Calories { get; set; } public string? ImageUrl { get; set; } }
public record CreateMenuItemResponse(Guid Id, string Name, string Sku, bool Succeeded, IEnumerable<string>? Errors);
public class UpdateMenuItemCommand { public Guid MenuItemId { get; set; } public string? Name { get; set; } public string? Description { get; set; } public string? Slug { get; set; } public string? Sku { get; set; } public double? Price { get; set; } public Guid? CategoryId { get; set; } public int? Calories { get; set; } public string? ImageUrl { get; set; } }
public record UpdateMenuItemResponse(Guid Id, string Name, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteMenuItemResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
public record ActivateMenuItemResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record DeactivateMenuItemResponse(Guid Id, bool Active, bool Succeeded, IEnumerable<string>? Errors);
public record SetMenuItemAvailabilityResponse(Guid Id, bool IsAvailable, bool Succeeded, IEnumerable<string>? Errors);
public record AddIngredientToMenuItemResponse(bool Succeeded, IEnumerable<string>? Errors);
public record RemoveIngredientFromMenuItemResponse(bool Succeeded, IEnumerable<string>? Errors);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus { Pending, Confirmed, InPreparation, Ready, InDelivery, Completed, Cancelled, Failed }

public class Address
{
    public Guid StreetTypeId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public Guid NeighborhoodId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType { Cpf, Cnpj, Passport }

public record OrderItemExtraCommand(Guid IngredientId, string Name, double? Price);
public record OrderItemCommand(Guid MenuItemId, int Quantity, string? Note, IEnumerable<OrderItemExtraCommand>? Extras);
public class CreateOrderCommand 
{ 
    public CreateOrderCommand() { }
    public CreateOrderCommand(Guid customerId, Address deliveryAddress, double deliveryFee, IEnumerable<OrderItemCommand> items, string? couponCode)
    {
        CustomerId = customerId; DeliveryAddress = deliveryAddress; DeliveryFee = deliveryFee; Items = items; CouponCode = couponCode;
    }
    public Guid CustomerId { get; set; } 
    public Address DeliveryAddress { get; set; } = new(); 
    public double DeliveryFee { get; set; } 
    public IEnumerable<OrderItemCommand> Items { get; set; } = []; 
    public string? CouponCode { get; set; } 
}
public record CreateOrderResponse(Guid OrderId, double TotalItemsPrice, double TotalDiscounts, double FinalAmount, bool Succeeded, IEnumerable<string>? Errors);

public record OrderItemDetailDto(Guid IngredientId, string Name, double Price);
public record OrderItemDto(Guid Id, Guid MenuItemId, string ProductName, string Sku, double UnitPrice, int Quantity, double TotalItemPrice, string? Note, IEnumerable<OrderItemDetailDto> Details);
public record OrderDiscountDto(string Name, double Amount, string? CouponCode);

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
    public Address DeliveryAddress { get; set; } = new();
    public double DeliveryFee { get; set; }
    public double TotalItemsPrice { get; set; }
    public double TotalDiscounts { get; set; }
    public double FinalAmount { get; set; }
    public IEnumerable<OrderItemDto> Items { get; set; } = [];
    public IEnumerable<OrderDiscountDto> Discounts { get; set; } = [];
}

public record GetAllOrdersResponse(IEnumerable<OrderDto> Items, int TotalCount, int Page, int PageSize);
public record GetOrderByIdResponse(OrderDto? Order);
public record GetOrdersByCustomerResponse(IEnumerable<OrderDto> Items, int TotalCount, int Page, int PageSize);
public record GetOrdersByStatusResponse(IEnumerable<OrderDto> Items, int TotalCount, int Page, int PageSize);
public class UpdateOrderStatusCommand 
{ 
    public UpdateOrderStatusCommand() { }
    public UpdateOrderStatusCommand(Guid orderId, OrderStatus status, string? cancelReason)
    {
        OrderId = orderId; Status = status; CancelReason = cancelReason;
    }
    public Guid OrderId { get; set; } 
    public OrderStatus Status { get; set; } 
    public string? CancelReason { get; set; } 
}
public record UpdateOrderStatusResponse(Guid OrderId, OrderStatus NewStatus, bool Succeeded, IEnumerable<string>? Errors);
public record CancelOrderResponse(Guid OrderId, bool Succeeded, IEnumerable<string>? Errors);

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public double Value { get; set; }
    public bool IsPercentage { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public double? MinimumOrderValue { get; set; }
    public bool Active { get; set; }
}

public class CreateCouponCommand { public string Code { get; set; } = string.Empty; public double Value { get; set; } public bool IsPercentage { get; set; } public DateTimeOffset ExpirationDate { get; set; } public int? UsageLimit { get; set; } public double? MinimumOrderValue { get; set; } }
public record CreateCouponResponse(Guid Id, string Code, bool Succeeded, IEnumerable<string>? Errors);
public record GetAllCouponsResponse(IEnumerable<CouponDto> Items, int TotalCount, int Page, int PageSize);
public record GetCouponByIdResponse(CouponDto? Coupon);
public record GetCouponByCodeResponse(CouponDto? Coupon);
public record GetActiveCouponsResponse(IEnumerable<CouponDto> Items, int TotalCount, int Page, int PageSize);
public class UpdateCouponCommand { public Guid CouponId { get; set; } public string? Code { get; set; } public double? Value { get; set; } public bool? IsPercentage { get; set; } public DateTimeOffset? ExpirationDate { get; set; } public int? UsageLimit { get; set; } public double? MinimumOrderValue { get; set; } public bool? Active { get; set; } }
public record UpdateCouponResponse(Guid Id, string Code, bool Succeeded, IEnumerable<string>? Errors);
public record CancelCouponResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);

public record Error(string Code, string Message);

// --- Checkout Calculate DTOs ---
public record CheckoutCalculationQuery(
    IReadOnlyList<CheckoutItemDto> Items,
    string? CouponCode = null,
    decimal? DeliveryFee = null
);

public record CheckoutItemDto(
    Guid MenuItemId,
    int Quantity,
    string? Note = null,
    IReadOnlyList<CheckoutExtraDto>? Extras = null
);

public record CheckoutExtraDto(
    Guid IngredientId,
    string Name,
    double? Price
);

public record CheckoutCalculationResponse(
    decimal Subtotal,
    decimal DeliveryFee,
    decimal DiscountAmount,
    decimal Total,
    IReadOnlyList<CheckoutItemResponseDto> Items,
    DiscountDto? AppliedDiscount
);

public record CheckoutItemResponseDto(
    Guid MenuItemId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalItemPrice,
    string? Note,
    IReadOnlyList<CheckoutExtraDto> Extras
);

public record DiscountDto(
    string Name,
    decimal Amount,
    string? CouponCode
);

public record ViaCepAddressDto(
    string ZipCode,
    string Street,
    string Neighborhood,
    string City,
    string State
);

// --- Identity DTOs ---
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = [];
}

public record GetAllUsersResponse(IEnumerable<UserDto> Items, int TotalCount, int Page, int PageSize);
public record GetUserResponse(UserDto? User);
public record CreateUserResponse(Guid UserId, string Email, bool Succeeded, IEnumerable<string>? Errors);
public record UpdateUserResponse(Guid UserId, string Email, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteUserResponse(Guid UserId, bool Succeeded, IEnumerable<string>? Errors);
public record GetAllRolesResponse(IEnumerable<string> Roles);
public record CreateRoleResponse(string Name, bool Succeeded, IEnumerable<string>? Errors);
public record AssignRoleToUserResponse(bool Succeeded, IEnumerable<string>? Errors);
public record RemoveRoleFromUserResponse(bool Succeeded, IEnumerable<string>? Errors);

// --- Customer Profile DTOs ---
public record GetCustomerProfileResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string? ProfilePictureUrl,
    bool IsActive,
    DocumentDto Document,
    AddressDto DeliveryAddress,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record DocumentDto(
    string Number,
    DocumentType DocumentType
);

public record AddressDto(
    Guid StreetTypeId,
    string StreetName,
    string Number,
    Guid NeighborhoodId,
    string ZipCode,
    string Complement
);

public class UpdateCustomerProfileCommand
{
    public Guid IdentityId { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateCustomerProfileResponse
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

public class UpdateCustomerDocumentCommand
{
    public Guid CustomerProfileId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
}

public class UpdateCustomerDocumentResponse
{
    public Guid Id { get; set; }
    public string? DocumentNumber { get; set; }
    public DocumentType DocumentType { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

public class UpdateCustomerAddressCommand
{
    public Guid CustomerProfileId { get; set; }
    public Guid StreetTypeId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public Guid NeighborhoodId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string? Complement { get; set; }
}

public class UpdateCustomerAddressResponse
{
    public Guid Id { get; set; }
    public Guid StreetTypeId { get; set; }
    public string? StreetName { get; set; }
    public string? Number { get; set; }
    public Guid NeighborhoodId { get; set; }
    public string? ZipCode { get; set; }
    public string? Complement { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

public class UpdateCustomerBirthDateCommand
{
    public Guid CustomerProfileId { get; set; }
    public DateTime? BirthDate { get; set; }
}

public class UpdateCustomerBirthDateResponse
{
    public Guid Id { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
