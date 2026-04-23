using Flunt.Notifications;

namespace GoodHamburger.Shared.ValueObjects.Base;

public abstract class ValueObject : Notifiable<Notification>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        var other = obj as ValueObject;
        return other != null && 
               GetEqualityComponents()
                   .SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    public abstract void Validate();
}