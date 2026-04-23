using System.Text;
using System.Text.Json.Serialization;
using GoodHamburger.Application.Extensions;
using GoodHamburger.Database.Extensions;
using GoodHamburger.Infrastructure.Extensions;
using GoodHamburger.Observability.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace GoodHamburger.Api.Configuration;

public static class AppConfiguration
{
    public static void AddGlobalConfiguration(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        services.AddOpenApi()
            .AddEndpointsApiExplorer()
            .AddCors()
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddSwaggerConfiguration(configuration)
            .AddDatabaseConfiguration(configuration)
            .AddApplicationServicesConfiguration()
            .AddRepositoriesConfiguration()
            .AddAuthenticationConfiguration(configuration)
            .AddSre(configuration, builder);
    }
    
    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
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
        
        return services;
    }
    
    private static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["JWT:Key"] ?? "";
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var validateIssuerSigningKey = bool.Parse(configuration["JWT:ValidateIssuerSigningKey"] ?? "");
        var validateIssuer = bool.Parse(configuration["JWT:ValidateIssuer"] ?? "");
        var validateAudience = bool.Parse(configuration["JWT:ValidateAudience"] ?? "");
        var issuer = configuration["JWT:Issuer"] ?? "";
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = validateIssuer,
                ValidateAudience = validateAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = validateIssuerSigningKey,
                ValidIssuer = issuer,
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = symmetricSecurityKey,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        return services;
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
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors(options =>
        {
            options.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
        
        app.MapControllers();
    } 
}