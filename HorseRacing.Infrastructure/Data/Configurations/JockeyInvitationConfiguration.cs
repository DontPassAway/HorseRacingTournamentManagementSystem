using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class JockeyInvitationConfiguration : IEntityTypeConfiguration<JockeyInvitation>
{
    public void Configure(EntityTypeBuilder<JockeyInvitation> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Message).HasMaxLength(500);
        builder.Property(i => i.ResponseMessage).HasMaxLength(500);

        builder.HasOne(i => i.Horse)
               .WithMany(h => h.JockeyInvitations)
               .HasForeignKey(i => i.HorseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.HorseOwner)
               .WithMany()
               .HasForeignKey(i => i.HorseOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Jockey)
               .WithMany(j => j.ReceivedInvitations)
               .HasForeignKey(i => i.JockeyId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Race)
               .WithMany()
               .HasForeignKey(i => i.RaceId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
