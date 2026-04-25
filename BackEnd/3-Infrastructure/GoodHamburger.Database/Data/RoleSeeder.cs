using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Database.Data;

public interface IRoleSeeder
{
    Task SeedRolesAsync();
}

public class RoleSeeder : IRoleSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<RoleSeeder> _logger;

    private static readonly List<SeedRole> DefaultRoles =
    [
        new SeedRole
        {
            Name = "Admin",
            Description = "The main role. Admin role can configure and administrate all resources of the application. The highest level"
        },
        new SeedRole
        {
            Name = "User",
            Description = "The basic role. User role can only use the application. The lowest level"
        },
        new SeedRole
        {
            Name = "Manager",
            Description = "An intermediate role. Manager role can administrate  resources of the application. An intermediate level"
        },
        new SeedRole
        {
            Name = "Employee",
            Description = "An intermediate role. Employee role can operate  resources of the application. An intermediate level, lowest than Manager"
        }
    ];

    public RoleSeeder(RoleManager<ApplicationRole> roleManager, ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedRolesAsync()
    {
        foreach (var seedRole in DefaultRoles)
        {
            try
            {
                var existingRole = await _roleManager.FindByNameAsync(seedRole.Name);
                
                if (existingRole != null)
                {
                    _logger.LogInformation("Role {RoleName} already exists, skipping creation", seedRole.Name);
                    continue;
                }

                var role = new ApplicationRole(seedRole.Name, seedRole.Description)
                {
                    Id = seedRole.Id
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} created successfully with ID {RoleId}", seedRole.Name, seedRole.Id);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create role {RoleName}: {Errors}", seedRole.Name, errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding role {RoleName}", seedRole.Name);
            }
        }
    }

    private class SeedRole
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
