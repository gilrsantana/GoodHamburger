using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Shared.Entities.Locations;

public class City : Entity
{
    public string Name { get; private set; } = null!;
    public Guid StateId { get; private set; }
    public State? State { get; private set; }
    public IReadOnlyCollection<Neighborhood>? Neighborhoods { get; private set; }

    protected City() { }
    
    private City(string name, Guid stateId)
    {
        Name = name;
        StateId = stateId;
        Validate();
    }
    
    public static City Create(string name, Guid stateId)
    {
        return new City(name.Trim(), stateId);
    }

    public City UpdateCityName(string name)
    {
        Name = name.Trim();
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<City>()
            .Requires()
            .IsNotNullOrEmpty(Name, "Name", "Nome da cidade é obrigatório.")
            .IsNotNullOrEmpty(StateId.ToString(), "StateId", "ID do estado é obrigatório.")
        );
    }
}