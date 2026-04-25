namespace GoodHamburger.Application.IdentityServices.Commands;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
);

public record RefreshTokenResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    bool Succeeded,
    IEnumerable<string> Errors
);
