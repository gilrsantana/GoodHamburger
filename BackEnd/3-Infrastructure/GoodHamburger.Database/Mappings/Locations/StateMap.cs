using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Locations;

public class StateMap : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("geo_states");
        builder.HasKey(s => s.Id);
        builder.Ignore(s => s.Notifications);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(s => s.UF)
            .IsRequired()
            .HasMaxLength(2)
            .HasColumnType("char(2)")
            .HasColumnName("uf");

        builder.Property(s => s.CountryId)
            .IsRequired()
            .HasColumnName("country_id");

        builder.HasOne(s => s.Country)
            .WithMany(c => c.States)
            .HasForeignKey(s => s.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.UF, s.CountryId }).IsUnique();
    }
}