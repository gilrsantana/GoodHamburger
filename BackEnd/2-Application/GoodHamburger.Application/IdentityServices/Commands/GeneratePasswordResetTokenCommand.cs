namespace GoodHamburger.Application.IdentityServices.Commands;

public record GeneratePasswordResetTokenCommand(
    string UserId
);

public record GeneratePasswordResetTokenResponse(
    string UserId,
    string Token,
    bool Succeeded,
    IEnumerable<string> Errors
);
