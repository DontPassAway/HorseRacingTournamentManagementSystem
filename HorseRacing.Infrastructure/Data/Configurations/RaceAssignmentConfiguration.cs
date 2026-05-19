using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class RaceAssignmentConfiguration : IEntityTypeConfiguration<RaceAssignment>
{
    public void Configure(EntityTypeBuilder<RaceAssignment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.RaceId, a.RefereeUserId }).IsUnique();

        builder.HasOne(a => a.Race)
               .WithMany(r => r.RaceAssignments)
               .HasForeignKey(a => a.RaceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.RefereeUser)
               .WithMany(u => u.RaceAssignments)
               .HasForeignKey(a => a.RefereeUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
