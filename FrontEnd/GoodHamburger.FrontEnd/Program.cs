using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Fluxor;
using GoodHamburger.FrontEnd;
using GoodHamburger.FrontEnd.Clients;
using GoodHamburger.FrontEnd.Services;
using Microsoft.AspNetCore.Components.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add MudBlazor Services (Must be before HttpClient for Snackbar injection)
builder.Services.AddMudServices();

// Register Auth Services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Global HTTP Interceptor (handles auth headers and error handling)
builder.Services.AddTransient<GlobalHttpHandler>();

// Framework HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// API Client using the Global Handler
builder.Services.AddHttpClient<IGoodHamburgerClient, GoodHamburgerClient>(client => 
{
    client.BaseAddress = new Uri("http://localhost:5275");
})
.AddHttpMessageHandler<GlobalHttpHandler>();

// Fluxor
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
});

await builder.Build().RunAsync();
