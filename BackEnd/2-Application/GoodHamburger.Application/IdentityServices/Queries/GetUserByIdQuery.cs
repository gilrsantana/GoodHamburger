namespace GoodHamburger.Application.IdentityServices.Queries;

public record GetUserByIdQuery(string UserId);

public record GetUserResponse(
    Guid Id,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    IEnumerable<string> Roles,
    IEnumerable<(string Type, string Value)> Claims
);
