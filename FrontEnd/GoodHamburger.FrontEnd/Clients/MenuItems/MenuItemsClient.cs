using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.MenuItems;

public interface IMenuItemsClient
{
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
}

public class MenuItemsClient : IMenuItemsClient
{
    private readonly HttpClient _httpClient;

    public MenuItemsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<GetAllMenuItemsResponse> GetAllMenuItemsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetAllMenuItemsResponse>($"api/MenuItems?page={page}&pageSize={pageSize}");
    
    public Task<GetMenuItemByIdResponse> GetMenuItemByIdAsync(Guid id) 
        => GetAsync<GetMenuItemByIdResponse>($"api/MenuItems/{id}");
    
    public Task<GetMenuItemBySkuResponse> GetMenuItemBySkuAsync(string sku) 
        => GetAsync<GetMenuItemBySkuResponse>($"api/MenuItems/sku/{sku}");
    
    public Task<GetMenuItemsByCategoryResponse> GetMenuItemsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10) 
        => GetAsync<GetMenuItemsByCategoryResponse>($"api/MenuItems/category/{categoryId}?page={page}&pageSize={pageSize}");
    
    public Task<GetActiveMenuItemsResponse> GetActiveMenuItemsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetActiveMenuItemsResponse>($"api/MenuItems/active?page={page}&pageSize={pageSize}");
    
    public Task<GetAvailableMenuItemsResponse> GetAvailableMenuItemsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetAvailableMenuItemsResponse>($"api/MenuItems/available?page={page}&pageSize={pageSize}");
    
    public Task<SearchMenuItemsResponse> SearchMenuItemsAsync(string searchTerm, int page = 1, int pageSize = 10) 
        => GetAsync<SearchMenuItemsResponse>($"api/MenuItems/search?searchTerm={searchTerm}&page={page}&pageSize={pageSize}");
    
    public Task<GetMenuItemsByPriceRangeResponse> GetMenuItemsByPriceRangeAsync(double minPrice, double maxPrice, int page = 1, int pageSize = 10) 
        => GetAsync<GetMenuItemsByPriceRangeResponse>($"api/MenuItems/price-range?minPrice={minPrice}&maxPrice={maxPrice}&page={page}&pageSize={pageSize}");
    
    public Task<CreateMenuItemResponse> CreateMenuItemAsync(CreateMenuItemCommand command) 
        => PostAsync<CreateMenuItemResponse>("api/MenuItems", command);
    
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
    
    public Task<ActivateMenuItemResponse> ActivateMenuItemAsync(Guid id) 
        => PostAsync<ActivateMenuItemResponse>($"api/MenuItems/{id}/activate", null);
    
    public Task<DeactivateMenuItemResponse> DeactivateMenuItemAsync(Guid id) 
        => PostAsync<DeactivateMenuItemResponse>($"api/MenuItems/{id}/deactivate", null);
    
    public Task<SetMenuItemAvailabilityResponse> SetMenuItemAvailabilityAsync(Guid id, bool isAvailable) 
        => PostAsync<SetMenuItemAvailabilityResponse>($"api/MenuItems/{id}/availability?isAvailable={isAvailable}", null);
    
    public Task<AddIngredientToMenuItemResponse> AddIngredientToMenuItemAsync(Guid menuItemId, Guid ingredientId, bool isRemovable = true) 
        => PostAsync<AddIngredientToMenuItemResponse>($"api/MenuItems/{menuItemId}/ingredients/{ingredientId}?isRemovable={isRemovable}", null);
    
    public Task<RemoveIngredientFromMenuItemResponse> RemoveIngredientFromMenuItemAsync(Guid menuItemId, Guid ingredientId) 
        => DeleteAsync<RemoveIngredientFromMenuItemResponse>($"api/MenuItems/{menuItemId}/ingredients/{ingredientId}");

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
}
