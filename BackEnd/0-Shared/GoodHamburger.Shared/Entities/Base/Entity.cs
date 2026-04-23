using Flunt.Notifications;

namespace GoodHamburger.Shared.Entities.Base;

public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; } = null;
    
    public abstract void Validate();
}