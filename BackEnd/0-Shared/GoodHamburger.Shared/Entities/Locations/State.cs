using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Shared.Entities.Locations;

public class State : Entity
{
    public string Name { get; private set; } = null!;
    public string UF { get; private set; } = null!;
    public Guid CountryId { get; private set; }
    public Country? Country { get; private set; }
    public IReadOnlyCollection<City>? Cities { get; private set; }

    protected State() { }
    
    private State(string name, string uf, Guid countryId)
    {
        Name = name;
        UF = uf.ToUpper();
        CountryId = countryId;
        
        var contract = new Contract<State>()
            .IsNotNullOrEmpty(name, "Name", "Name is a required field.")
            .IsNotNullOrEmpty(uf, "UF", "UF is a required field.");
        
        AddNotifications(contract);
    }
    
    public static State Create(string name, string uf, Guid countryId)
    {
        return new State(name.Trim(), uf.Trim(), countryId);
    }

    public State UpdateName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public State UpdateUf(string uf)
    {
        UF = uf.ToUpper();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<State>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Name", "Nome do estado é obrigatório.")
            .IsNotNullOrEmpty(UF, "UF", "UF do estado é obrigatório.")
            .IsNotNullOrEmpty(CountryId.ToString(), "CountryId", "ID do país é obrigatório."));
    }
}