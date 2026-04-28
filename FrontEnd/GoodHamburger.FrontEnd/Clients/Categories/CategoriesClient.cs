using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.Categories;

public interface ICategoriesClient
{
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
}

public class CategoriesClient : ICategoriesClient
{
    private readonly HttpClient _httpClient;

    public CategoriesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<GetAllCategoriesResponse> GetAllCategoriesAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetAllCategoriesResponse>($"api/Categories?page={page}&pageSize={pageSize}");
    
    public Task<GetCategoryByIdResponse> GetCategoryByIdAsync(Guid id) 
        => GetAsync<GetCategoryByIdResponse>($"api/Categories/{id}");
    
    public Task<GetCategoryBySlugResponse> GetCategoryBySlugAsync(string slug) 
        => GetAsync<GetCategoryBySlugResponse>($"api/Categories/slug/{slug}");
    
    public Task<GetCategoriesByTypeResponse> GetCategoriesByTypeAsync(MenuCategoryType type, int page = 1, int pageSize = 10) 
        => GetAsync<GetCategoriesByTypeResponse>($"api/Categories/type/{type}?page={page}&pageSize={pageSize}");
    
    public Task<GetActiveCategoriesResponse> GetActiveCategoriesAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetActiveCategoriesResponse>($"api/Categories/active?page={page}&pageSize={pageSize}");
    
    public Task<CreateCategoryResponse> CreateCategoryAsync(CreateCategoryCommand command) 
        => PostAsync<CreateCategoryResponse>("api/Categories", command);
    
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
    
    public Task<ActivateCategoryResponse> ActivateCategoryAsync(Guid id) 
        => PostAsync<ActivateCategoryResponse>($"api/Categories/{id}/activate", null);
    
    public Task<DeactivateCategoryResponse> DeactivateCategoryAsync(Guid id) 
        => PostAsync<DeactivateCategoryResponse>($"api/Categories/{id}/deactivate", null);

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
