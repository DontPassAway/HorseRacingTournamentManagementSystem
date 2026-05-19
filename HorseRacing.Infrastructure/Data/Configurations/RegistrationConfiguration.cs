using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Race)
               .WithMany(race => race.Registrations)
               .HasForeignKey(r => r.RaceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Horse)
               .WithMany(h => h.Registrations)
               .HasForeignKey(r => r.HorseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.HorseOwner)
               .WithMany()
               .HasForeignKey(r => r.HorseOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Jockey)
               .WithMany(j => j.Registrations)
               .HasForeignKey(r => r.JockeyId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.RaceResult)
               .WithOne(rr => rr.Registration)
               .HasForeignKey<RaceResult>(rr => rr.RegistrationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
