using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Locations;

public class StreetTypeMap : IEntityTypeConfiguration<StreetType>
{
    public void Configure(EntityTypeBuilder<StreetType> builder)
    {
        builder.ToTable("geo_street_types");
        builder.HasKey(st => st.Id);
        builder.Ignore(st => st.Notifications);

        builder.Property(st => st.Name)
            .IsRequired()
            .HasMaxLength(50) 
            .HasColumnName("name");

        builder.Property(st => st.Abbreviation)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnName("abbreviation");
    }
}