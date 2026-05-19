using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class BetConfiguration : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.SpectatorUser)
               .WithMany(u => u.Bets)
               .HasForeignKey(b => b.SpectatorUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Race)
               .WithMany(r => r.Bets)
               .HasForeignKey(b => b.RaceId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.PredictedHorse)
               .WithMany()
               .HasForeignKey(b => b.PredictedHorseId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
