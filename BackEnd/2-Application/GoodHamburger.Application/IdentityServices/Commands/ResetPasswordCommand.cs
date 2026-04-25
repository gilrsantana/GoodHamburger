namespace GoodHamburger.Application.IdentityServices.Commands;

public record ResetPasswordCommand(
    string UserId,
    string Token,
    string NewPassword
);

public record ResetPasswordResponse(
    string UserId,
    bool Succeeded,
    IEnumerable<string> Errors
);
