using Flunt.Validations;
using GoodHamburger.Shared.ValueObjects.Base;

namespace GoodHamburger.Shared.ValueObjects.Contacts;

public class Email : ValueObject
{
    public string Address { get; private set; }
    
    private Email() { }
    
    public static Email Create(string address)
    {
        return new Email(address?.Trim().ToLower() ?? string.Empty);
    }
    
    public Email(string address)
    {
        Address = address?.Trim().ToLower() ?? string.Empty;
        Validate();
    }

    public void UpdateAddress(string address)
    {
        Address = address?.Trim().ToLower() ?? string.Empty;
        Validate();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<Email>()
            .Requires()
            .IsEmail(Address, "Email.Address", "O e-mail informado é inválido")
        );
    }

    public override string ToString() => Address;
}