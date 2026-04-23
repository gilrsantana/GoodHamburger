using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Shared.Entities.Locations;

public class StreetType : Entity
{
    public string Name { get; private set; }
    public string Abbreviation { get; private set; }

    private StreetType(string name, string abbreviation)
    {
        Name = name;
        Abbreviation = abbreviation;
        
        Validate();
    }
    
    public static StreetType Create(string name, string abbreviation)
    {
        return new StreetType(name.Trim(), abbreviation.Trim());
    }

    public StreetType UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public StreetType UpdateAbbreviation(string abbreviation)
    {
        Abbreviation = abbreviation.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<StreetType>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Name", "Name is a required field.")
            .IsNotNullOrEmpty(Abbreviation, "Abbreviation", "Abbreviation is a required field."));
    }
}