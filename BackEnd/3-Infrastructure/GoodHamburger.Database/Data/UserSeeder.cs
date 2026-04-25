using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Database.Data;

public interface IUserSeeder
{
    Task SeedUsersAsync();
}

public class UserSeeder : IUserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<UserSeeder> _logger;

    private static readonly List<SeedUser> DefaultUsers =
    [
        new SeedUser
        {
            Email = "admin@goodhamburger.com",
            UserName = "Admin",
            Password = "Admin@123456",
            FirstName = "Admin",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0001",
            RoleName = "Admin"
        },
        new SeedUser
        {
            Email = "manager@goodhamburger.com",
            UserName = "Manager",
            Password = "Manager@123456",
            FirstName = "Manager",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0002",
            RoleName = "Manager"
        },
        new SeedUser
        {
            Email = "employee@goodhamburger.com",
            UserName = "Employee",
            Password = "Employee@123456",
            FirstName = "Employee",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0003",
            RoleName = "Employee"
        },
        new SeedUser
        {
            Email = "user@goodhamburger.com",
            UserName = "User",
            Password = "User@123456",
            FirstName = "User",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0004",
            RoleName = "User"
        }
    ];

    public UserSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedUsersAsync()
    {
        foreach (var seedUser in DefaultUsers)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(seedUser.Email);
                
                if (existingUser != null)
                {
                    _logger.LogInformation("User {UserName} already exists with email {Email}, skipping creation", seedUser.UserName, seedUser.Email);
                    continue;
                }

                var roleExists = await _roleManager.RoleExistsAsync(seedUser.RoleName);
                if (!roleExists)
                {
                    _logger.LogWarning("Role {RoleName} does not exist, cannot assign to user {UserName}", seedUser.RoleName, seedUser.UserName);
                    continue;
                }

                var user = new ApplicationUser
                {
                    UserName = seedUser.UserName,
                    Email = seedUser.Email,
                    EmailConfirmed = true,
                    PhoneNumber = seedUser.PhoneNumber,
                    PhoneNumberConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, seedUser.Password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create user {UserName}: {Errors}", seedUser.UserName, errors);
                    continue;
                }

                _logger.LogInformation("User {UserName} created successfully with ID {UserId}", seedUser.UserName, user.Id);

                var roleResult = await _userManager.AddToRoleAsync(user, seedUser.RoleName);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} assigned to user {UserName} successfully", seedUser.RoleName, seedUser.UserName);
                }
                else
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to assign role {RoleName} to user {UserName}: {Errors}", seedUser.RoleName, seedUser.UserName, errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding user {UserName}", seedUser.UserName);
            }
        }
    }

    private class SeedUser
    {
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public required string RoleName { get; set; }
    }
}
