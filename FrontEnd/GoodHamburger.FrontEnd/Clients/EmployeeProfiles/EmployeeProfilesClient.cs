using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.EmployeeProfiles;

public interface IEmployeeProfilesClient
{
    Task<CreateEmployeeProfileResponse> CreateEmployeeProfileAsync(CreateEmployeeProfileCommand command);
    Task<UpdateEmployeeProfileResponse> UpdateEmployeeProfileAsync(Guid id, UpdateEmployeeProfileCommand command);
    Task<UpdateEmployeeCodeResponse> UpdateEmployeeCodeAsync(Guid id, UpdateEmployeeCodeCommand command);
    Task<UpdateEmployeeRoleTitleResponse> UpdateEmployeeRoleTitleAsync(Guid id, UpdateEmployeeRoleTitleCommand command);
    Task<DeleteEmployeeProfileResponse> DeleteEmployeeProfileAsync(Guid id);
    Task<GetEmployeeProfileByIdResponse> GetEmployeeProfileByIdAsync(Guid id);
    Task<GetEmployeeProfileByIdentityIdResponse> GetEmployeeProfileByIdentityIdAsync(Guid identityId);
    Task<GetAllEmployeeProfilesResponse> GetAllEmployeeProfilesAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<GetActiveEmployeeProfilesResponse> GetActiveEmployeeProfilesAsync(int page = 1, int pageSize = 10);
}

public class EmployeeProfilesClient : IEmployeeProfilesClient
{
    private readonly HttpClient _httpClient;

    public EmployeeProfilesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<CreateEmployeeProfileResponse> CreateEmployeeProfileAsync(CreateEmployeeProfileCommand command) 
        => PostAsync<CreateEmployeeProfileResponse>("api/EmployeeProfiles", command);
    
    public async Task<UpdateEmployeeProfileResponse> UpdateEmployeeProfileAsync(Guid id, UpdateEmployeeProfileCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/EmployeeProfiles/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateEmployeeProfileResponse>() ?? throw new InvalidOperationException();
    }
    
    public async Task<UpdateEmployeeCodeResponse> UpdateEmployeeCodeAsync(Guid id, UpdateEmployeeCodeCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/EmployeeProfiles/{id}/code", command);
        return await response.Content.ReadFromJsonAsync<UpdateEmployeeCodeResponse>() ?? throw new InvalidOperationException();
    }
    
    public async Task<UpdateEmployeeRoleTitleResponse> UpdateEmployeeRoleTitleAsync(Guid id, UpdateEmployeeRoleTitleCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/EmployeeProfiles/{id}/role-title", command);
        return await response.Content.ReadFromJsonAsync<UpdateEmployeeRoleTitleResponse>() ?? throw new InvalidOperationException();
    }
    
    public async Task<DeleteEmployeeProfileResponse> DeleteEmployeeProfileAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/EmployeeProfiles/{id}");
        return await response.Content.ReadFromJsonAsync<DeleteEmployeeProfileResponse>() ?? throw new InvalidOperationException();
    }
    
    public Task<GetEmployeeProfileByIdResponse> GetEmployeeProfileByIdAsync(Guid id) 
        => GetAsync<GetEmployeeProfileByIdResponse>($"api/EmployeeProfiles/{id}");
    
    public Task<GetEmployeeProfileByIdentityIdResponse> GetEmployeeProfileByIdentityIdAsync(Guid identityId) 
        => GetAsync<GetEmployeeProfileByIdentityIdResponse>($"api/EmployeeProfiles/by-identity/{identityId}");
    
    public Task<GetAllEmployeeProfilesResponse> GetAllEmployeeProfilesAsync(int page = 1, int pageSize = 10, string? search = null) 
        => GetAsync<GetAllEmployeeProfilesResponse>($"api/EmployeeProfiles?page={page}&pageSize={pageSize}&search={search}");
    
    public Task<GetActiveEmployeeProfilesResponse> GetActiveEmployeeProfilesAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetActiveEmployeeProfilesResponse>($"api/EmployeeProfiles/active?page={page}&pageSize={pageSize}");

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
