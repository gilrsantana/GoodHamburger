using Flunt.Br;
using Flunt.Br.Extensions;
using GoodHamburger.Shared.ValueObjects.Base;
using GoodHamburger.Shared.ValueObjects.Documents.Enums;

namespace GoodHamburger.Shared.ValueObjects.Documents.Base;

public class Document : ValueObject
{
    public string Number { get; private set; } = null!;
    public DocumentType DocumentType { get; private set; }
    
    protected Document() { }
    
    private Document(string number, DocumentType documentType)
    {
        if (string.IsNullOrWhiteSpace(number))
            AddNotification("Document", "Number is required");

        Number = number;
        DocumentType = documentType;
        ValidateDocument(documentType);
    }
    
    public static Document Create(string number, DocumentType documentType)
    {
        return new Document(number, documentType);
    }

    public void Update(string number, DocumentType documentType)
    {
        Number = number;
        DocumentType = documentType;
        ValidateDocument(documentType);
    }

    private void ValidateDocument(DocumentType documentType)
    {
        if (ValidationStrategies.TryGetValue(documentType, out var validationAction))
        {
            validationAction(this);
        }
        else
        {
            throw new ArgumentException($"Unsupported document type: {documentType}");
        }
    }
    
    private static readonly Dictionary<DocumentType, Action<Document>> ValidationStrategies = new()
    {
        { DocumentType.Cpf, document => document.ValidateCpf() },
        { DocumentType.Cnpj, document => document.ValidateCnpj() },
        { DocumentType.Passport, document => document.ValidatePassport() },
        { DocumentType.MunicipalRegistration, document => document.ValidateMunicipalRegistration() },
        { DocumentType.StateRegistration, document => document.ValidateStateRegistration() }
    };

    private void ValidateCnpj()
    {
        var contract =  new Contract()
            .IsCnpj(Number, "Number", "Invalid CNPJ" );
        AddNotifications(contract);
    }
    
    private void ValidateCpf()
    {
        var contract =  new Contract()
            .IsCpf(Number, "Number", "Invalid CPF");
        AddNotifications(contract);
    }
    
    private void ValidatePassport()
    {
        var contract =  new Contract()
            .IsPassport(Number, "Number", "Invalid Passport");
        AddNotifications(contract);
    }
    
    private void ValidateMunicipalRegistration()
    {
        var contract =  new Contract()
            .IsNotNullOrEmpty(Number, "Number", "Invalid Municipal Registration");
        AddNotifications(contract);
    }
    
    private void ValidateStateRegistration()
    {
        var contract =  new Contract()
            .IsNotNullOrEmpty(Number, "Number", "Invalid State Registration");
        AddNotifications(contract);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
        yield return GetType().Name; 
    }

    public sealed override void Validate()
    {
        ValidateDocument(DocumentType);
    }
}