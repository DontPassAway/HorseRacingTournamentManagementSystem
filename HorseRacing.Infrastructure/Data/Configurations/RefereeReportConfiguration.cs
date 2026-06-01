using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class RefereeReportConfiguration : IEntityTypeConfiguration<RefereeReport>
{
    public void Configure(EntityTypeBuilder<RefereeReport> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Content).IsRequired().HasMaxLength(5000);
        builder.Property(r => r.ViolationDescription).HasMaxLength(1000);

        builder.HasOne(r => r.Race)
               .WithMany(race => race.RefereeReports)
               .HasForeignKey(r => r.RaceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.RefereeUser)
               .WithMany(u => u.RefereeReports)
               .HasForeignKey(r => r.RefereeUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ViolatingRegistration)
               .WithMany()
               .HasForeignKey(r => r.ViolatingRegistrationId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
