using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface ICreateRoleHandler
{
    Task<Result<CreateRoleResponse>> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default);
}

public class CreateRoleHandler : ICreateRoleHandler
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<CreateRoleHandler> _logger;

    public CreateRoleHandler(RoleManager<ApplicationRole> roleManager, ILogger<CreateRoleHandler> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<CreateRoleResponse>> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = new ApplicationRole(command.RoleName, command.Description);

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} created successfully", command.RoleName);
                
                return Result<CreateRoleResponse>.Success(new CreateRoleResponse(
                    role.Id,
                    role.Name!,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Role creation failed: {Errors}", string.Join(", ", errors));

            return Result<CreateRoleResponse>.Failure(
                new Error("Role.CreationFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating role {RoleName}", command.RoleName);
            return Result<CreateRoleResponse>.Failure(
                new Error("Role.CreationError", "An error occurred while creating the role")
            );
        }
    }
}
