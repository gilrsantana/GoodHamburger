using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.Ingredients;

public interface IIngredientsClient
{
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
}

public class IngredientsClient : IIngredientsClient
{
    private readonly HttpClient _httpClient;

    public IngredientsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<GetAllIngredientsResponse> GetAllIngredientsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetAllIngredientsResponse>($"api/Ingredients?page={page}&pageSize={pageSize}");
    
    public Task<GetIngredientByIdResponse> GetIngredientByIdAsync(Guid id) 
        => GetAsync<GetIngredientByIdResponse>($"api/Ingredients/{id}");
    
    public Task<GetActiveIngredientsResponse> GetActiveIngredientsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetActiveIngredientsResponse>($"api/Ingredients/active?page={page}&pageSize={pageSize}");
    
    public Task<SearchIngredientsResponse> SearchIngredientsAsync(string searchTerm, int page = 1, int pageSize = 10) 
        => GetAsync<SearchIngredientsResponse>($"api/Ingredients/search?searchTerm={searchTerm}&page={page}&pageSize={pageSize}");
    
    public Task<GetIngredientsByPriceRangeResponse> GetIngredientsByPriceRangeAsync(double minPrice, double maxPrice, int page = 1, int pageSize = 10) 
        => GetAsync<GetIngredientsByPriceRangeResponse>($"api/Ingredients/price-range?minPrice={minPrice}&maxPrice={maxPrice}&page={page}&pageSize={pageSize}");
    
    public Task<CreateIngredientResponse> CreateIngredientAsync(CreateIngredientCommand command) 
        => PostAsync<CreateIngredientResponse>("api/Ingredients", command);
    
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
    
    public Task<ActivateIngredientResponse> ActivateIngredientAsync(Guid id) 
        => PostAsync<ActivateIngredientResponse>($"api/Ingredients/{id}/activate", null);
    
    public Task<DeactivateIngredientResponse> DeactivateIngredientAsync(Guid id) 
        => PostAsync<DeactivateIngredientResponse>($"api/Ingredients/{id}/deactivate", null);

    private async Task<T> PostAsync<T>(string url, object? body)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response body is null");
    }

    private async Task<T> GetAsync<T>(string url)
    {
        return await _httpClient.GetFromJsonAsync<T>(url) ?? throw new InvalidOperationException("Response body is null");
    }
}
