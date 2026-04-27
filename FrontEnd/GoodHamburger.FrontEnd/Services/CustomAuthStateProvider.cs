using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using GoodHamburger.FrontEnd.Clients;
using Microsoft.JSInterop;

namespace GoodHamburger.FrontEnd.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";
    private const string AddressKey = "userAddress";

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try 
        {
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(savedToken);
            
            if (loginResponse == null || loginResponse.ExpiresAt < DateTimeOffset.UtcNow)
            {
                 return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(loginResponse.AccessToken).ToList();
            
            // Explicitly add the UserId from loginResponse as a claim if it's missing from JWT
            if (!claims.Any(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier))
            {
                claims.Add(new Claim("userId", loginResponse.UserId.ToString()));
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task MarkUserAsAuthenticated(LoginResponse loginResponse)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, JsonSerializer.Serialize(loginResponse));
        
        var claims = ParseClaimsFromJwt(loginResponse.AccessToken).ToList();
        if (!claims.Any(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier))
        {
            claims.Add(new Claim("userId", loginResponse.UserId.ToString()));
        }

        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AddressKey);
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task SaveAddressAsync(string addressJson)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AddressKey, addressJson);
    }

    public async Task<string?> LoadAddressAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AddressKey);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var parts = jwt.Split('.');
        if (parts.Length < 2) return claims;

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        claims.Add(new Claim(kvp.Key, item.ToString() ?? ""));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
                }
            }
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
