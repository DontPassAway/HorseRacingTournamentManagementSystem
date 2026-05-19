using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class RaceResultConfiguration : IEntityTypeConfiguration<RaceResult>
{
    public void Configure(EntityTypeBuilder<RaceResult> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PrizeMoney).HasPrecision(18, 2);

        builder.HasOne(r => r.Race)
               .WithMany(race => race.RaceResults)
               .HasForeignKey(r => r.RaceId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.ConfirmedBy)
               .WithMany()
               .HasForeignKey(r => r.ConfirmedByUserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
