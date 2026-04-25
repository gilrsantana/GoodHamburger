using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Queries;

public interface IGetAllRolesHandler
{
    Task<Result<GetAllRolesResponse>> HandleAsync(GetAllRolesQuery query, CancellationToken cancellationToken = default);
}

public class GetAllRolesHandler : IGetAllRolesHandler
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllRolesHandler> _logger;

    public GetAllRolesHandler(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<GetAllRolesHandler> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<GetAllRolesResponse>> HandleAsync(GetAllRolesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = _roleManager.Roles.AsEnumerable();
            var roleResponses = new List<RoleResponse>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                var userIds = usersInRole.Select(u => u.Id);

                roleResponses.Add(new RoleResponse(
                    role.Id,
                    role.Name!,
                    userIds
                ));
            }

            var response = new GetAllRolesResponse(roleResponses);

            _logger.LogInformation("Retrieved {RoleCount} roles successfully", roleResponses.Count);
            return Result<GetAllRolesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving roles");
            return Result<GetAllRolesResponse>.Failure(
                new Error("Role.RetrievalError", "An error occurred while retrieving roles")
            );
        }
    }
}
