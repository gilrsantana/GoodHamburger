using Microsoft.Extensions.Configuration;

namespace GoodHamburger.Database.Configuration;

public class DatabaseConfigurationBinding
{
    public static DatabaseConfigurationDefinition Bind(IConfiguration configuration)
    {
        var section = configuration.GetSection(DatabaseConfigurationDefinition.SectionName);
        var definition = section.Get<DatabaseConfigurationDefinition>() ?? new DatabaseConfigurationDefinition();
        var connectionString = configuration.GetConnectionString(DatabaseConfigurationDefinition.ConnectionStringName) ?? "";
        definition.ConnectionString = connectionString;
        return definition;
    }
}