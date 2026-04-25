namespace GoodHamburger.Application.IdentityServices.Queries;

public record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null
);

public record GetAllUsersResponse(
    IEnumerable<GetUserResponse> Users,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
