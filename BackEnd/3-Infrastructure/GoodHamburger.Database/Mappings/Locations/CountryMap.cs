using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Locations;

public class CountryMap : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("geo_countries");
        builder.HasKey(c => c.Id);
        builder.Ignore(c => c.Notifications);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(c => c.IsoCode)
            .IsRequired()
            .HasMaxLength(3)
            .HasColumnType("char(3)") 
            .HasColumnName("iso_code");

        builder.HasIndex(c => c.IsoCode).IsUnique();
    }
}