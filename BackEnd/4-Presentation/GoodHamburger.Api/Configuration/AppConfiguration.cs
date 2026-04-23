using GoodHamburger.Application.Extensions;
using GoodHamburger.Database.Extensions;
using Microsoft.OpenApi;

namespace GoodHamburger.Api.Configuration;

public static class AppConfiguration
{
    public static void AddGlobalConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddDatabaseConfiguration(configuration);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfiguration(configuration);
        services.AddApplicationServicesConfiguration();
    }
    
    private static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GoodHamburger API",
                Version = "v1",
                Description = "API para gestão de hamburgueria, estoque e pedidos."
            });
        });
    }

    public static void LoadApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            
        } 
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoodHamburger API V1");
            c.RoutePrefix = "swagger";
        });
        
        app.UseHttpsRedirection();
    } 
}