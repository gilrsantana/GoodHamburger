namespace GoodHamburger.FrontEnd.Clients.EmployeeProfiles;

public class CreateEmployeeProfileCommand
{
    public Guid IdentityId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
}

public record CreateEmployeeProfileResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string EmployeeCode,
    string RoleTitle,
    bool IsActive,
    bool Succeeded,
    IEnumerable<string>? Errors
);

public class UpdateEmployeeProfileCommand
{
    public Guid EmployeeProfileId { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool? IsActive { get; set; }
}

public record UpdateEmployeeProfileResponse(
    Guid Id,
    string FullName,
    string DisplayName,
    bool IsActive,
    bool Succeeded,
    IEnumerable<string>? Errors
);

public class UpdateEmployeeCodeCommand
{
    public Guid EmployeeProfileId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
}

public record UpdateEmployeeCodeResponse(
    Guid Id,
    string EmployeeCode,
    bool Succeeded,
    IEnumerable<string>? Errors
);

public class UpdateEmployeeRoleTitleCommand
{
    public Guid EmployeeProfileId { get; set; }
    public string RoleTitle { get; set; } = string.Empty;
}

public record UpdateEmployeeRoleTitleResponse(
    Guid Id,
    string RoleTitle,
    bool Succeeded,
    IEnumerable<string>? Errors
);

public record DeleteEmployeeProfileResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string>? Errors
);

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

public record GetAllEmployeeProfilesResponse(
    IEnumerable<EmployeeProfileDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetActiveEmployeeProfilesResponse(
    IEnumerable<EmployeeProfileDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
