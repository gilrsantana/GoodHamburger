using GoodHamburger.Domain.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Accounts;

public class EmployeeProfileMap : IEntityTypeConfiguration<EmployeeProfile>
{
    public void Configure(EntityTypeBuilder<EmployeeProfile> builder)
    {
        builder.ToTable("employee_profiles");
        builder.HasKey(ep => ep.Id);

        builder.Ignore(ep => ep.Notifications);

        builder.Property(ep => ep.IdentityId)
            .IsRequired()
            .HasColumnName("identity_id");

        builder.Property(ep => ep.FullName)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("full_name");

        builder.Property(ep => ep.DisplayName)
            .IsRequired()
            .HasMaxLength(60)
            .HasColumnName("display_name");

        builder.Property(ep => ep.ProfilePictureUrl)
            .HasMaxLength(255)
            .HasColumnName("profile_picture_url");

        builder.Property(ep => ep.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(ep => ep.EmployeeCode)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("employee_code");

        builder.Property(ep => ep.RoleTitle)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("role_title");

        builder.HasIndex(ep => ep.IdentityId)
            .IsUnique()
            .HasDatabaseName("ix_employee_profile_identity_id");

        builder.HasIndex(ep => ep.EmployeeCode)
            .IsUnique()
            .HasDatabaseName("ix_employee_profile_code");
    }
}