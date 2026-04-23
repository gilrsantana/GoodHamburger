using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Locations;

public class NeighborhoodMap : IEntityTypeConfiguration<Neighborhood>
{
    public void Configure(EntityTypeBuilder<Neighborhood> builder)
    {
        builder.ToTable("geo_neighborhoods");
        builder.HasKey(n => n.Id);
        builder.Ignore(n => n.Notifications);

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("name");

        builder.Property(n => n.CityId)
            .IsRequired()
            .HasColumnName("city_id");

        builder.HasOne(n => n.City)
            .WithMany(c => c.Neighborhoods)
            .HasForeignKey(n => n.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.CityId);
    }
}