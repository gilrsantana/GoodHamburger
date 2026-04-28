using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.Auth;

public interface IAuthClient
{
    Task<LoginResponse> LoginAsync(LoginCommand command);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenCommand command);
    Task<LogoutResponse> LogoutAsync();
}

public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<LoginResponse> LoginAsync(LoginCommand command) => PostAsync<LoginResponse>("api/Auth/login", command);
    public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenCommand command) => PostAsync<RefreshTokenResponse>("api/Auth/refresh-token", command);
    
    public async Task<LogoutResponse> LogoutAsync()
    {
        var response = await _httpClient.PostAsync("api/Auth/logout", null);
        return await response.Content.ReadFromJsonAsync<LogoutResponse>() ?? throw new InvalidOperationException();
    }

    private async Task<T> PostAsync<T>(string url, object? body)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response body is null");
    }
}
