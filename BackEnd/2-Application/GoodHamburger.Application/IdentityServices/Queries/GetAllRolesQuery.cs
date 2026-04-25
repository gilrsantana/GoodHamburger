namespace GoodHamburger.Application.IdentityServices.Queries;

public record GetAllRolesQuery();

public record GetAllRolesResponse(
    IEnumerable<RoleResponse> Roles
);

public record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    IEnumerable<Guid> UserIds
);
