using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class JockeyProfileConfiguration : IEntityTypeConfiguration<JockeyProfile>
{
    public void Configure(EntityTypeBuilder<JockeyProfile> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Weight).HasPrecision(8, 2);
        builder.Property(j => j.LicenseNumber).HasMaxLength(50);
        builder.Property(j => j.Nationality).HasMaxLength(100);
    }
}
