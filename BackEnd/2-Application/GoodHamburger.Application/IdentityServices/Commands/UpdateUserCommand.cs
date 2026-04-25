namespace GoodHamburger.Application.IdentityServices.Commands;

public record UpdateUserCommand(
    string UserId,
    string? Email = null,
    string? UserName = null,
    string? PhoneNumber = null,
    bool? EmailConfirmed = null
);

public record UpdateUserResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);
