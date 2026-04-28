using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using GoodHamburger.FrontEnd.Clients.Auth;
using Microsoft.JSInterop;
using MudBlazor;

namespace GoodHamburger.FrontEnd.Services;

public class GlobalHttpHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ISnackbar _snackbar;
    private const string TokenKey = "authToken";

    public GlobalHttpHandler(IJSRuntime jsRuntime, ISnackbar snackbar)
    {
        _jsRuntime = jsRuntime;
        _snackbar = snackbar;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Add Authorization Header only if token is valid and not expired
        var savedToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
        if (!string.IsNullOrWhiteSpace(savedToken))
        {
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(savedToken);
            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                // Check if token is expired
                if (loginResponse.ExpiresAt > DateTimeOffset.UtcNow)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }
                else
                {
                    // Token is expired, clear it from localStorage
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                }
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Global Error Handling
        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    // Clear expired/invalid token
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                    _snackbar.Add("Sessão expirada. Por favor, faça login novamente.", Severity.Warning);
                    break;
                case HttpStatusCode.Forbidden:
                    _snackbar.Add("Você não tem permissão para realizar esta ação.", Severity.Error);
                    break;
                case HttpStatusCode.BadRequest:
                    // Usually handled locally in the page, but can show a generic message if empty
                    break;
                case HttpStatusCode.InternalServerError:
                    _snackbar.Add("Erro interno no servidor. Tente novamente mais tarde.", Severity.Error);
                    break;
            }
        }

        return response;
    }
}
