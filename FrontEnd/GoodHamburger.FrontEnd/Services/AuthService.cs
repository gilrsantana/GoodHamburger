using GoodHamburger.FrontEnd.Clients.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace GoodHamburger.FrontEnd.Services;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginCommand loginCommand);
    Task Logout();
    Task<string?> RefreshToken();
}

public class AuthService : IAuthService
{
    private readonly IAuthClient _client;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(IAuthClient client, 
                       AuthenticationStateProvider authenticationStateProvider)
    {
        _client = client;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<LoginResponse> Login(LoginCommand loginCommand)
    {
        var loginResponse = await _client.LoginAsync(loginCommand);

        if (loginResponse.Succeeded)
        {
            await ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResponse);
        }

        return loginResponse;
    }

    public async Task Logout()
    {
        await _client.LogoutAsync();
        await ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    }

    public async Task<string?> RefreshToken()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        var token = user.FindFirst("AccessToken")?.Value;
        var refreshToken = user.FindFirst("RefreshToken")?.Value;

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            return null;

        var response = await _client.RefreshTokenAsync(new RefreshTokenCommand(token, refreshToken));

        if (response.Succeeded)
        {
            await ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(new LoginResponse(response.UserId, response.AccessToken, response.RefreshToken, response.ExpiresAt, true, null!));
            return response.AccessToken;
        }

        return null;
    }
}
