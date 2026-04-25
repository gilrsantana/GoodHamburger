namespace GoodHamburger.Application.IdentityServices.Commands;

public record DeleteUserCommand(string UserId);

public record DeleteUserResponse(
    string Id,
    bool Succeeded,
    IEnumerable<string> Errors
);
