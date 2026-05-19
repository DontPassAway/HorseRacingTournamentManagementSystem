using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class PrizeConfiguration : IEntityTypeConfiguration<Prize>
{
    public void Configure(EntityTypeBuilder<Prize> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();

        builder.HasOne(p => p.Tournament)
               .WithMany(t => t.Prizes)
               .HasForeignKey(p => p.TournamentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
