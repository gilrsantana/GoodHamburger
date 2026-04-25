namespace GoodHamburger.Application.AccountServices.Queries;

public record GetEmployeeProfileByIdQuery(Guid EmployeeProfileId);

public record GetEmployeeProfileByIdResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string EmployeeCode,
    string RoleTitle,
    string? ProfilePictureUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record GetEmployeeProfileByIdentityIdQuery(Guid IdentityId);

public record GetEmployeeProfileByIdentityIdResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string EmployeeCode,
    string RoleTitle,
    string? ProfilePictureUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record GetAllEmployeeProfilesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null
);

public record GetAllEmployeeProfilesResponse(
    IReadOnlyList<EmployeeProfileDto> Employees,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record GetActiveEmployeeProfilesQuery(
    int Page = 1,
    int PageSize = 10
);

public record GetActiveEmployeeProfilesResponse(
    IReadOnlyList<EmployeeProfileDto> Employees,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record EmployeeProfileDto(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string EmployeeCode,
    string RoleTitle,
    string? ProfilePictureUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
