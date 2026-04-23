using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Shared.Entities.Locations;

public class Neighborhood : Entity
{
    public string Name { get; private set; }  = null!;
    public Guid CityId { get; private set; }
    public City City { get; private set; }

    protected Neighborhood() { }
    
    private Neighborhood(string name, Guid cityId)
    {
        Name = name;
        CityId = cityId;
        
        Validate();
    }
    
    public static Neighborhood Create(string name, Guid cityId)
    {
        return new Neighborhood(name.Trim(), cityId);
    }

    public Neighborhood UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }
    
    

    public sealed override void Validate()
    {
        AddNotifications(new Contract<Neighborhood>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Name", "Nome do bairro é obrigatório.")
            .IsNotNullOrEmpty(CityId.ToString(), "CityId", "ID da cidade é obrigatório."));
    }
}