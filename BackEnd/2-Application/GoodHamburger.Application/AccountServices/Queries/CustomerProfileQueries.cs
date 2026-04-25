using GoodHamburger.Shared.ValueObjects.Documents.Enums;

namespace GoodHamburger.Application.AccountServices.Queries;

public record GetCustomerProfileByIdQuery(Guid CustomerProfileId);

public record GetCustomerProfileByIdResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string? ProfilePictureUrl,
    bool IsActive,
    DocumentDto Document,
    AddressDto DeliveryAddress,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record GetCustomerProfileByIdentityIdQuery(Guid IdentityId);

public record GetCustomerProfileByIdentityIdResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string? ProfilePictureUrl,
    bool IsActive,
    DocumentDto Document,
    AddressDto DeliveryAddress,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record GetCustomerProfileByDocumentQuery(string DocumentNumber);

public record GetCustomerProfileByDocumentResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string? ProfilePictureUrl,
    bool IsActive,
    DocumentDto Document,
    AddressDto DeliveryAddress,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record GetAllCustomerProfilesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null
);

public record GetAllCustomerProfilesResponse(
    IReadOnlyList<CustomerProfileDto> Customers,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record GetActiveCustomerProfilesQuery(
    int Page = 1,
    int PageSize = 10
);

public record GetActiveCustomerProfilesResponse(
    IReadOnlyList<CustomerProfileDto> Customers,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record CustomerProfileDto(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string? ProfilePictureUrl,
    bool IsActive,
    DocumentDto Document,
    AddressDto DeliveryAddress,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record DocumentDto(
    string Number,
    DocumentType DocumentType
);

public record AddressDto(
    Guid StreetTypeId,
    string StreetName,
    string Number,
    Guid NeighborhoodId,
    string ZipCode,
    string Complement
);
