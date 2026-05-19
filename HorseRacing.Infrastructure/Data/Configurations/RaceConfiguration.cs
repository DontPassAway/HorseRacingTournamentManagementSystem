using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class RaceConfiguration : IEntityTypeConfiguration<Race>
{
    public void Configure(EntityTypeBuilder<Race> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Distance).HasPrecision(10, 2);

        builder.HasOne(r => r.Tournament)
               .WithMany(t => t.Races)
               .HasForeignKey(r => r.TournamentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
