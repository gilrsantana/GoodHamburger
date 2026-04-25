using Flunt.Br;
using GoodHamburger.Shared.Entities.Locations;
using GoodHamburger.Shared.ValueObjects.Base;

namespace GoodHamburger.Shared.ValueObjects.Locations;

public class Address : ValueObject
{
    public Guid StreetTypeId { get; private set; } 
    public string StreetName { get; private set; } = null!;
    public string Number { get; private set; } = null!;
    public string Complement { get; private set; } = null!;
    public string ZipCode { get; private set; } = null!;
    public Guid NeighborhoodId { get; private set; }

    public virtual StreetType? StreetType { get; private set; }
    public virtual Neighborhood? Neighborhood { get; private set; }

    protected Address() { }
    
    private Address(Guid streetTypeId, 
        string streetName, 
        string number, 
        Guid neighborhoodId, 
        string zipCode, 
        string complement = "")
    {
        StreetTypeId = streetTypeId;
        StreetName = streetName;
        Number = number;
        NeighborhoodId = neighborhoodId;
        ZipCode = zipCode;
        Complement = complement;
        
        Validate();
    }

    public static Address Create(Guid streetTypeId,
        string streetName,
        string number,
        Guid neighborhoodId,
        string zipCode,
        string complement = "")
    {
        return new Address(streetTypeId, streetName.Trim(), number.Trim(), neighborhoodId, zipCode.Trim(), complement.Trim());
    }

    public void Update(Guid streetTypeId,
        string streetName,
        string number,
        Guid neighborhoodId,
        string zipCode,
        string complement = "")
    {
        StreetTypeId = streetTypeId;
        StreetName = streetName;
        Number = number;
        NeighborhoodId = neighborhoodId;
        ZipCode = zipCode;
        Complement = complement;
        Validate();
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetTypeId;
        yield return StreetName;
        yield return Number;
        yield return NeighborhoodId;
        yield return ZipCode;
    }

    public sealed override void Validate()
    {
        var contract = new Contract();
        contract
            .Requires()
            .IsNotNullOrEmpty(StreetName, "StreetName", "Street name is required")
            .IsNotNullOrEmpty(Number, "Number", "Number is required")
            .IsNotNullOrEmpty(ZipCode, "ZipCode", "Zip code is required");

        AddNotifications(contract);
    }
}