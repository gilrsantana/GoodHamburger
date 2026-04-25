namespace GoodHamburger.Application.IdentityServices.Commands;

public record AddClaimToUserCommand(
    string UserId,
    string ClaimType,
    string ClaimValue
);

public record AddClaimToUserResponse(
    string UserId,
    string ClaimType,
    string ClaimValue,
    bool Succeeded,
    IEnumerable<string> Errors
);
