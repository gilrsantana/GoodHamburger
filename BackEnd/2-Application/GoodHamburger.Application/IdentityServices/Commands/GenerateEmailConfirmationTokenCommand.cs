namespace GoodHamburger.Application.IdentityServices.Commands;

public record GenerateEmailConfirmationTokenCommand(
    string UserId
);

public record GenerateEmailConfirmationTokenResponse(
    string UserId,
    string Token,
    bool Succeeded,
    IEnumerable<string> Errors
);
