namespace GoodHamburger.Application.IdentityServices.Commands;

public record CreateRoleCommand(
    string RoleName,
    string? Description = null
);

public record CreateRoleResponse(
    Guid Id,
    string Name,
    bool Succeeded,
    IEnumerable<string> Errors
);
