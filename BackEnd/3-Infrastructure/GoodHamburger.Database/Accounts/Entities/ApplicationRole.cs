using Microsoft.AspNetCore.Identity;

namespace GoodHamburger.Database.Accounts.Entities;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public override Guid Id { get; set; } = Guid.CreateVersion7();
    public string? Description { get; set; }

    public ApplicationRole() { }

    public ApplicationRole(string name) : base(name) { }
}