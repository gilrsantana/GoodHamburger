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

builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddTransient<GlobalHttpHandler>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? 
    (builder.HostEnvironment.IsDevelopment() ? "http://localhost:5275" : "http://localhost:8500");

builder.Services.AddHttpClient<IGoodHamburgerClient, GoodHamburgerClient>(client => 
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<GlobalHttpHandler>();

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
});

await builder.Build().RunAsync();
