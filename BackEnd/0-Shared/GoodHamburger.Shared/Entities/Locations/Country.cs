using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Shared.Entities.Locations;

public class Country : Entity
{
    public string Name { get; private set; } = null!;
    public string IsoCode { get; private set; } = null!;
    public IReadOnlyCollection<State>? States { get; private set; }

    protected Country() { }
    
    private Country(string name, string isoCode)
    {
        Name = name;
        IsoCode = isoCode.ToUpper();
        
        Validate();
    }
    
    public static Country Create(string name, string isoCode)
    {
        return new Country(name.Trim(), isoCode.Trim());
    }

    public Country UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Country UpdateIsoCode(string isoCode)
    {
        IsoCode = isoCode.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<Country>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Name", "Nome do país é obrigatório.")
            .IsNotNullOrEmpty(IsoCode, "IsoCode", "Código ISO do país é obrigatório.")
        );
    }
}