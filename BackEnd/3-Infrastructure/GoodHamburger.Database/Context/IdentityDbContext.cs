using GoodHamburger.Infrastructure.Accounts.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Database.Context;

public class IdentityDbContext : IdentityDbContext<
    ApplicationUser, 
    ApplicationRole, 
    Guid, 
    ApplicationUserClaim, 
    ApplicationUserRole, 
    ApplicationUserLogin, 
    ApplicationRoleClaim, 
    ApplicationUserToken>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<ApplicationUser>(e => e.ToTable("identity_users"));
        builder.Entity<ApplicationRole>(e => e.ToTable("identity_roles"));
        builder.Entity<ApplicationUserRole>(e => e.ToTable("identity_user_roles"));
        builder.Entity<ApplicationUserClaim>(e => e.ToTable("identity_user_claims"));
        builder.Entity<ApplicationRoleClaim>(e => e.ToTable("identity_role_claims"));
        builder.Entity<ApplicationUserLogin>(e => e.ToTable("identity_user_logins"));
        builder.Entity<ApplicationUserToken>(e => e.ToTable("identity_user_tokens"));
    }
}