using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.CustomerProfiles;

public interface ICustomerProfilesClient
{
    Task<GetCustomerProfileResponse> GetCustomerProfileAsync(Guid identityId);
    Task<UpdateCustomerProfileResponse> UpdateCustomerProfileAsync(Guid id, UpdateCustomerProfileCommand command);
    Task<UpdateCustomerDocumentResponse> UpdateCustomerDocumentAsync(Guid id, UpdateCustomerDocumentCommand command);
    Task<UpdateCustomerAddressResponse> UpdateCustomerAddressAsync(Guid id, UpdateCustomerAddressCommand command);
    Task<UpdateCustomerBirthDateResponse> UpdateCustomerBirthDateAsync(Guid id, UpdateCustomerBirthDateCommand command);
}

public class CustomerProfilesClient : ICustomerProfilesClient
{
    private readonly HttpClient _httpClient;

    public CustomerProfilesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<GetCustomerProfileResponse> GetCustomerProfileAsync(Guid identityId) 
        => GetAsync<GetCustomerProfileResponse>($"api/CustomerProfiles/by-identity/{identityId}");
    
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

    private async Task<T> GetAsync<T>(string url)
    {
        return await _httpClient.GetFromJsonAsync<T>(url) ?? throw new InvalidOperationException("Response body is null");
    }
}
