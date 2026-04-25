namespace GoodHamburger.Application.IdentityServices.Commands;

public record LoginCommand(
    string Email,
    string Password
);

public record LoginResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    bool Succeeded,
    IEnumerable<string> Errors
);
