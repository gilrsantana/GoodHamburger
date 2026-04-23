using System.ComponentModel;
using System.Runtime.InteropServices;
using Flunt.Br;
using Flunt.Validations;
using GoodHamburger.Shared.ValueObjects.Documents.Base;
using GoodHamburger.Shared.ValueObjects.Documents.Enums;
using GoodHamburger.Shared.ValueObjects.Locations;

namespace GoodHamburger.Domain.Accounts.Entities;

public class CustomerProfile : AccountProfile
{
    public Document Document { get; private set; } = null!;
    public DateTime? BirthDate { get; private set; }
    public Address DeliveryAddress { get; private set; } = null!;

    private readonly DocumentType[] _availableDocumentTypes =
        [DocumentType.Cpf, DocumentType.Cnpj, DocumentType.Passport];
    
    protected CustomerProfile() { }
    
    private CustomerProfile(
        Guid identityId, 
        string fullName, 
        string displayName, 
        Document document, 
        Address deliveryAddress,
        DateTime? birthDate = null,
        string? profilePictureUrl = null) 
        : base(identityId, fullName, displayName, profilePictureUrl)
    {
        Document = document;
        DeliveryAddress = deliveryAddress;
        BirthDate = birthDate;
        Validate();
    }

    public static CustomerProfile Create(Guid identityId, 
        string fullName, 
        string displayName, 
        Document document, 
        Address deliveryAddress,
        DateTime? birthDate = null)
    {
        var customer = new CustomerProfile(identityId, fullName.Trim(), displayName.Trim(), document, deliveryAddress, birthDate);
        return customer;
    }

    public CustomerProfile UpdateFullNameCustomer(string fullName)
    {
        UpdateFullName(fullName);
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public CustomerProfile UpdateDisplayNameCustomer(string displayName)
    {
        UpdateDisplayName(displayName);
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public CustomerProfile UpdateProfilePictureCustomer(string url)
    {
        UpdateProfilePicture(url);
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public CustomerProfile UpdateActiveCustomer(bool active)
    {
        if (IsActive != active)
            ToggleActivate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    public CustomerProfile UpdateDocument(Document document)
    {
        if (!_availableDocumentTypes.Contains(document.DocumentType))
        {
            AddNotification("CustomerProfile.Document", "Customer must have a fiscal identifier (CPF/CNPJ/Passport).");
        }
        Document = document;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public CustomerProfile UpdateDeliveryAddress(Address deliveryAddress)
    {
        DeliveryAddress = deliveryAddress;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    public CustomerProfile UpdateBirthDate(DateTime? birthDate)
    {
        BirthDate = birthDate;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public sealed override void Validate()
    {
        AddNotifications(Document, DeliveryAddress);
        
        AddNotifications(new Contract<CustomerProfile>()
            .Requires()
            .IsNotNullOrEmpty(FullName, "FullName", "Deve informar o nome completo.")
            .IsNotNullOrEmpty(DisplayName, "DisplayName", "Deve informar o nome de exibição.")
            .IsUrlOrEmpty(ProfilePictureUrl, "ProfilePictureUrl", "Deve informar uma URL válida.")
            .AreNotEquals(IdentityId, Guid.Empty, "IdentityId", "Deve informar o ID de Identidade do cliente.")
        );
        
        if (!_availableDocumentTypes.Contains(Document.DocumentType))
            AddNotification("CustomerProfile.Document", "Deve informar um documento válido (CPF/CNPJ/Passport).");
        
        if (BirthDate >= DateTime.Now && BirthDate != null)
            AddNotification("CustomerProfile.BirthDate", "Data de nascimento inválida.");
    }
}