namespace GoodHamburger.Application.IdentityServices.Commands;

public record RemoveRoleFromUserCommand(
    string UserId,
    string RoleName
);

public record RemoveRoleFromUserResponse(
    string UserId,
    string RoleName,
    bool Succeeded,
    IEnumerable<string> Errors
);
