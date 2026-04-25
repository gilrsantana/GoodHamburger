namespace GoodHamburger.Application.IdentityServices.Commands;

public record CreateUserCommand(
    string Email,
    string UserName,
    string Password,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null
);

public record CreateUserResponse(
    Guid Id,
    string Email,
    string UserName,
    bool Succeeded,
    IEnumerable<string> Errors
);
