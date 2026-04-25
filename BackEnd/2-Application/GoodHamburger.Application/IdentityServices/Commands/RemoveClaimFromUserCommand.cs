namespace GoodHamburger.Application.IdentityServices.Commands;

public record RemoveClaimFromUserCommand(
    string UserId,
    string ClaimType,
    string ClaimValue
);

public record RemoveClaimFromUserResponse(
    string UserId,
    string ClaimType,
    string ClaimValue,
    bool Succeeded,
    IEnumerable<string> Errors
);
