using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class HorseConfiguration : IEntityTypeConfiguration<Horse>
{
    public void Configure(EntityTypeBuilder<Horse> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).IsRequired().HasMaxLength(100);
        builder.Property(h => h.Breed).IsRequired().HasMaxLength(100);
        builder.Property(h => h.Color).HasMaxLength(50);
        builder.Property(h => h.Weight).HasPrecision(8, 2);

        builder.HasOne(h => h.HorseOwner)
               .WithMany(o => o.Horses)
               .HasForeignKey(h => h.HorseOwnerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
