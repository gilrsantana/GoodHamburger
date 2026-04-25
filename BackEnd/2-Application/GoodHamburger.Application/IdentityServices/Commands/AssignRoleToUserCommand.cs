namespace GoodHamburger.Application.IdentityServices.Commands;

public record AssignRoleToUserCommand(
    string UserId,
    string RoleName
);

public record AssignRoleToUserResponse(
    string UserId,
    string RoleName,
    bool Succeeded,
    IEnumerable<string> Errors
);
