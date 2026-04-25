namespace GoodHamburger.Application.AccountServices.Commands;

public record CreateEmployeeProfileCommand(
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string EmployeeCode,
    string RoleTitle,
    string? ProfilePictureUrl = null
);

public record CreateEmployeeProfileResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string EmployeeCode,
    string RoleTitle,
    bool IsActive,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateEmployeeProfileCommand(
    Guid EmployeeProfileId,
    string? FullName = null,
    string? DisplayName = null,
    string? ProfilePictureUrl = null,
    bool? IsActive = null
);

public record UpdateEmployeeProfileResponse(
    Guid Id,
    string FullName,
    string DisplayName,
    bool IsActive,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateEmployeeCodeCommand(
    Guid EmployeeProfileId,
    string EmployeeCode
);

public record UpdateEmployeeCodeResponse(
    Guid Id,
    string EmployeeCode,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateEmployeeRoleTitleCommand(
    Guid EmployeeProfileId,
    string RoleTitle
);

public record UpdateEmployeeRoleTitleResponse(
    Guid Id,
    string RoleTitle,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeleteEmployeeProfileCommand(Guid EmployeeProfileId);

public record DeleteEmployeeProfileResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);
