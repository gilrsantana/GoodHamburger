namespace GoodHamburger.Database.Configuration;

public class DatabaseConfigurationDefinition
{
    public const string SectionName = "DatabaseSettings";
    public const string ConnectionStringName = "DefaultConnection";
    public string ConectionStringReference { get => ConnectionStringName; }
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelay { get; set; } = 5;
    public bool EnableSensitiveDataLogging { get; set; }
    public bool EnableDetailedErrors { get; set; }
    public string MigrationsAssembly { get; set; } = "";
    public DatabaseHealthCheckConfiguration HealthCheck { get; set; } = new();
}

public class DatabaseHealthCheckConfiguration
{
    public bool Enabled { get; set; } = true;
    public int Timeout { get; set; } = 5;
}