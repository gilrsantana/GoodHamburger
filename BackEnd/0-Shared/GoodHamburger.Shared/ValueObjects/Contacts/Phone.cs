using Flunt.Validations;
using GoodHamburger.Shared.ValueObjects.Base;

namespace GoodHamburger.Shared.ValueObjects.Contacts;

public class Phone : ValueObject
{
    public string CountryCode { get; private set; }
    public string AreaCode { get; private set; }
    public string Number { get; private set; }

    private Phone(string countryCode, string areaCode, string number)
    {
        CountryCode = countryCode?.Replace("+", "").Trim() ?? "55";
        AreaCode = areaCode?.Trim() ?? string.Empty;
        Number = number?.Trim() ?? string.Empty;

        Validate();
    }

    public static Phone Create(string countryCode, string areaCode, string number)
    {
        return new Phone(countryCode, areaCode, number);
    }
    
    public void Update(string countryCode, string areaCode, string number)
    {
        CountryCode = countryCode?.Replace("+", "").Trim() ?? "55";
        AreaCode = areaCode?.Trim() ?? string.Empty;
        Number = number?.Trim() ?? string.Empty;

        Validate();
    }
    
    public override string ToString() => $"+{CountryCode} ({AreaCode}) {Number}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return AreaCode;
        yield return Number;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<Phone>()
            .Requires()
            .IsNotNullOrEmpty(AreaCode, "PhoneNumber.AreaCode", "Empty area code")
            .IsNotNullOrEmpty(Number, "PhoneNumber.Number", "Empty number")
            .IsBetween(Number.Length, 8, 11, "PhoneNumber.Number", "Invalid number")
        );
    }
}