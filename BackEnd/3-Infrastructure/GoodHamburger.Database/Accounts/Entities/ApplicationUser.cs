using Microsoft.AspNetCore.Identity;

namespace GoodHamburger.Database.Accounts.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public override Guid Id { get; set; } = Guid.CreateVersion7();
}