using GoodHamburger.Database.Configuration;
using GoodHamburger.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Database.Extensions;

public static class DatabaseConfigurationExtensions
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfigurationDefinition>(
            configuration.GetSection(DatabaseConfigurationDefinition.SectionName));

        var dbConfig = new DatabaseConfigurationDefinition();
        configuration.GetSection(DatabaseConfigurationDefinition.SectionName).Bind(dbConfig);
        
        var connectionString = configuration.GetConnectionString(DatabaseConfigurationDefinition.ConnectionStringName);
        
        services.AddDbContext<IdentityDbContext>(options => 
            ConfigurePostgre(options, dbConfig, connectionString));
        
        services.AddDbContext<ApplicationDbContext>(options => 
            ConfigurePostgre(options, dbConfig, connectionString));

        return services;
    }
    
    private static void ConfigurePostgre(DbContextOptionsBuilder options, DatabaseConfigurationDefinition dbConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(dbConfig.CommandTimeout);
            
            if (dbConfig.MaxRetryCount > 0)
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: dbConfig.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(dbConfig.MaxRetryDelay),
                    errorCodesToAdd: null);
            }

            if (!string.IsNullOrWhiteSpace(dbConfig.MigrationsAssembly))
                npgsqlOptions.MigrationsAssembly(dbConfig.MigrationsAssembly);
        });

        options.EnableSensitiveDataLogging(dbConfig.EnableSensitiveDataLogging);
        options.EnableDetailedErrors(dbConfig.EnableDetailedErrors);
    }
}

