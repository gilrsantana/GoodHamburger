using System.Text;
using System.Text.Json.Serialization;
using GoodHamburger.Api.Middlewares;
using GoodHamburger.Application.Extensions;
using GoodHamburger.Database.Extensions;
using GoodHamburger.Infrastructure.Extensions;
using GoodHamburger.Observability.Extensions;
using GoodHamburger.Database.Accounts.Entities;
using GoodHamburger.Database.Context;
using GoodHamburger.Database.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        services.AddIdentityServices()
            .AddSwaggerConfiguration()
            .AddDatabaseConfiguration(configuration)
            .AddApplicationServicesConfiguration()
            .AddRepositoriesConfiguration()
            .AddAuthenticationConfiguration(configuration)
            .AddSre(configuration, builder);
        
        services.AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            
            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            
            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            // Admin-only policy for system configuration
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            // Policy for user management (Admin and Manager)
            options.AddPolicy("Management", policy =>
                policy.RequireRole("Admin", "Manager"));

            // Policy for operational tasks (Admin, Manager, Employee)
            options.AddPolicy("Operate", policy =>
                policy.RequireRole("Admin", "Manager", "Employee"));

            // Policy for all authenticated users including basic Users
            options.AddPolicy("Authenticated", policy =>
                policy.RequireAuthenticatedUser());
        });
        
        return services;
    }
    
    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GoodHamburger API",
                Version = "v1",
                Description = "API para gestão de hamburgueria, estoque e pedidos."
            });

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header. Enter just the token (without 'Bearer' prefix).",
            });
            
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
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
        var validAudience = configuration["JWT:Audience"];
        var issuer = configuration["JWT:Issuer"] ?? "";
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = validateIssuer,
                ValidateAudience = validateAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = validateIssuerSigningKey,
                ValidIssuer = issuer,
                ValidAudience = validAudience,
                IssuerSigningKey = symmetricSecurityKey,
                ClockSkew = TimeSpan.Zero
            };
            
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = 401,
                        Title = "Unauthorized",
                        Detail = "You must be authenticated to access this resource."
                    });
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = 403,
                        Title = "Forbidden",
                        Detail = "You do not have permission to perform this action."
                    });
                },
                OnAuthenticationFailed = async context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = 401,
                        Title = "Authentication Failed",
                        Detail = context.Exception.Message
                    });
                },
                OnTokenValidated = async context =>
                {
                    Console.WriteLine("Token validated successfully");
                    await Task.CompletedTask;
                }
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
            c.RoutePrefix = "";
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

        using (var scope = app.Services.CreateScope())
        {
            var roleSeeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();
            roleSeeder.SeedRolesAsync().GetAwaiter().GetResult();

            var userSeeder = scope.ServiceProvider.GetRequiredService<IUserSeeder>();
            userSeeder.SeedUsersAsync().GetAwaiter().GetResult();
        }
    } 
}