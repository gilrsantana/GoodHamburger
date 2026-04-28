using System.Text.Json.Serialization;

namespace GoodHamburger.FrontEnd.Clients.CustomerProfiles;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType { Cpf, Cnpj, Passport }

public record GetCustomerProfileResponse(
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

public class UpdateCustomerProfileCommand
{
    public Guid IdentityId { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateCustomerProfileResponse
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

public class UpdateCustomerDocumentCommand
{
    public Guid CustomerProfileId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
}

public class UpdateCustomerDocumentResponse
{
    public Guid Id { get; set; }
    public string? DocumentNumber { get; set; }
    public DocumentType DocumentType { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

public class UpdateCustomerAddressCommand
{
    public Guid CustomerProfileId { get; set; }
    public Guid StreetTypeId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public Guid NeighborhoodId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string? Complement { get; set; }
}

public class UpdateCustomerAddressResponse
{
    public Guid Id { get; set; }
    public Guid StreetTypeId { get; set; }
    public string? StreetName { get; set; }
    public string? Number { get; set; }
    public Guid NeighborhoodId { get; set; }
    public string? ZipCode { get; set; }
    public string? Complement { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

public class UpdateCustomerBirthDateCommand
{
    public Guid CustomerProfileId { get; set; }
    public DateTime? BirthDate { get; set; }
}

public class UpdateCustomerBirthDateResponse
{
    public Guid Id { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
