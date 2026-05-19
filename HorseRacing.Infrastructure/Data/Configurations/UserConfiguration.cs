using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorseRacing.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.Role).IsRequired();

        builder.HasOne(u => u.HorseOwnerProfile)
               .WithOne(h => h.User)
               .HasForeignKey<HorseOwner>(h => h.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.JockeyProfile)
               .WithOne(j => j.User)
               .HasForeignKey<JockeyProfile>(j => j.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
