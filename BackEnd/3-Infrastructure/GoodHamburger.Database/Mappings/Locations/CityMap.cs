using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Locations;

public class CityMap : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("geo_cities");
        builder.HasKey(c => c.Id);
        builder.Ignore(c => c.Notifications);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("name");

        builder.Property(c => c.StateId)
            .IsRequired()
            .HasColumnName("state_id");

        builder.HasOne(c => c.State)
            .WithMany(s => s.Cities)
            .HasForeignKey(c => c.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.StateId);
    }
}