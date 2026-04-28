using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.Identity;

public interface IIdentityClient
{
    Task<GetAllUsersResponse> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<GetUserResponse> GetUserByIdAsync(string id);
    Task<CreateUserResponse> CreateUserAsync(object command);
    Task<UpdateUserResponse> UpdateUserAsync(string id, object command);
    Task<DeleteUserResponse> DeleteUserAsync(string id);
    Task<GetAllRolesResponse> GetAllRolesAsync();
    Task<CreateRoleResponse> CreateRoleAsync(object command);
    Task<AssignRoleToUserResponse> AssignRoleToUserAsync(object command);
    Task<RemoveRoleFromUserResponse> RemoveRoleFromUserAsync(object command);
}

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient _httpClient;

    public IdentityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<GetAllUsersResponse> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null) 
        => GetAsync<GetAllUsersResponse>($"api/Users?page={page}&pageSize={pageSize}&search={search}");
    
    public Task<GetUserResponse> GetUserByIdAsync(string id) 
        => GetAsync<GetUserResponse>($"api/Users/{id}");
    
    public Task<CreateUserResponse> CreateUserAsync(object command) 
        => PostAsync<CreateUserResponse>("api/Users", command);
    
    public Task<UpdateUserResponse> UpdateUserAsync(string id, object command) 
        => PostAsync<UpdateUserResponse>($"api/Users/{id}", command);
    
    public Task<DeleteUserResponse> DeleteUserAsync(string id) 
        => PostAsync<DeleteUserResponse>($"api/Users/{id}", null);
    
    public Task<GetAllRolesResponse> GetAllRolesAsync() 
        => GetAsync<GetAllRolesResponse>("api/Roles");
    
    public Task<CreateRoleResponse> CreateRoleAsync(object command) 
        => PostAsync<CreateRoleResponse>("api/Roles", command);
    
    public Task<AssignRoleToUserResponse> AssignRoleToUserAsync(object command) 
        => PostAsync<AssignRoleToUserResponse>("api/Roles/assign", command);
    
    public Task<RemoveRoleFromUserResponse> RemoveRoleFromUserAsync(object command) 
        => PostAsync<RemoveRoleFromUserResponse>("api/Roles/remove", command);

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
