using GoodHamburger.Domain.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Accounts;

public class CustomerProfileMap : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("customer_profiles");
        builder.HasKey(cp => cp.Id);
        
        builder.Ignore(cp => cp.Notifications);

        builder.Property(cp => cp.IdentityId)
            .IsRequired()
            .HasColumnName("identity_id");

        builder.Property(cp => cp.FullName)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)")
            .HasColumnName("full_name");

        builder.Property(cp => cp.DisplayName)
            .IsRequired()
            .HasMaxLength(60)
            .HasColumnType("varchar(60)")
            .HasColumnName("display_name");

        builder.Property(cp => cp.ProfilePictureUrl)
            .HasMaxLength(255)
            .HasColumnName("profile_picture_url");

        builder.Property(cp => cp.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");
        
        builder.Property(cp => cp.BirthDate)
            .HasColumnType("date")
            .HasColumnName("birth_date");

        builder.OwnsOne(cp => cp.Document, doc =>
        {
            doc.Ignore(d => d.Notifications);
            doc.Property(d => d.Number)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .HasColumnName("document_number");
            
            doc.Property(d => d.DocumentType)
                .HasConversion<string>()
                .IsRequired()
                .HasColumnName("document_type");   
        });

        builder.OwnsOne(cp => cp.DeliveryAddress, addr =>
        {
            addr.Ignore(a => a.Notifications);

            addr.HasOne(a => a.StreetType)
                .WithMany()
                .HasForeignKey(a => a.StreetTypeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            addr.HasOne(a => a.Neighborhood)
                .WithMany()
                .HasForeignKey(a => a.NeighborhoodId)
                .OnDelete(DeleteBehavior.Restrict);

            addr.Property(a => a.StreetTypeId)
                .IsRequired()
                .HasColumnName("address_street_type_id");
            
            addr.Property(a => a.StreetName)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("address_street");

            addr.Property(a => a.Number)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("address_number");

            addr.Property(a => a.Complement)
                .HasMaxLength(100)
                .HasColumnName("address_complement");
            
            addr.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(8)
                .HasColumnType("char(8)")
                .HasColumnName("address_zip_code");
            
            addr.Property(a => a.NeighborhoodId)
                .IsRequired()
                .HasColumnName("address_neighborhood_id");
        });
        
        builder.HasIndex(cp => cp.IdentityId)
            .HasDatabaseName("ix_customer_profile_identity_id");

        builder.HasIndex(cp => cp.FullName)
            .HasDatabaseName("ix_customer_profile_full_name");
        
    }
}