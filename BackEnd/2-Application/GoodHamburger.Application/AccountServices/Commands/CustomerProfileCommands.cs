using GoodHamburger.Shared.ValueObjects.Documents.Enums;

namespace GoodHamburger.Application.AccountServices.Commands;

public record CreateCustomerProfileCommand(
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string DocumentNumber,
    DocumentType DocumentType,
    Guid StreetTypeId,
    string StreetName,
    string Number,
    Guid NeighborhoodId,
    string ZipCode,
    string Complement,
    DateTime? BirthDate = null,
    string? ProfilePictureUrl = null
);

public record CreateCustomerProfileResponse(
    Guid Id,
    Guid IdentityId,
    string FullName,
    string DisplayName,
    string DocumentNumber,
    DocumentType DocumentType,
    bool IsActive,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateCustomerProfileCommand(
    Guid IdentityId,
    string? FullName = null,
    string? DisplayName = null,
    string? ProfilePictureUrl = null,
    bool? IsActive = null
);

public record UpdateCustomerProfileResponse(
    Guid Id,
    string FullName,
    string DisplayName,
    bool IsActive,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateCustomerDocumentCommand(
    Guid IdentityId,
    string DocumentNumber,
    DocumentType DocumentType
);

public record UpdateCustomerDocumentResponse(
    Guid Id,
    string DocumentNumber,
    DocumentType DocumentType,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateCustomerAddressCommand(
    Guid IdentityId,
    Guid StreetTypeId,
    string StreetName,
    string Number,
    Guid NeighborhoodId,
    string ZipCode,
    string Complement
);

public record UpdateCustomerAddressResponse(
    Guid Id,
    Guid StreetTypeId,
    string StreetName,
    string Number,
    Guid NeighborhoodId,
    string ZipCode,
    string Complement,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateCustomerBirthDateCommand(
    Guid IdentityId,
    DateTime? BirthDate
);

public record UpdateCustomerBirthDateResponse(
    Guid Id,
    DateTime? BirthDate,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record DeleteCustomerProfileCommand(Guid CustomerProfileId);

public record DeleteCustomerProfileResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);
