using GoodHamburger.FrontEnd;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using GoodHamburger.FrontEnd.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? 
    (builder.HostEnvironment.IsDevelopment() ? "http://localhost:5275/" : "http://localhost:8500/");

builder.Services
    .AddMudBlazorServices()
    .AddAuthenticationServices()
    .AddHttpServices(apiBaseUrl)
    .AddFluxorServices();

await builder.Build().RunAsync();
