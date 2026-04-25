using GoodHamburger.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddGlobalConfiguration(builder.Configuration, builder);

var app = builder.Build();
app.LoadApplication();

app.Run();
