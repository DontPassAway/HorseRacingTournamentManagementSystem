using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<HorseOwner> HorseOwners => Set<HorseOwner>();
    public DbSet<JockeyProfile> JockeyProfiles => Set<JockeyProfile>();
    public DbSet<Horse> Horses => Set<Horse>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<RaceResult> RaceResults => Set<RaceResult>();
    public DbSet<Bet> Bets => Set<Bet>();
    public DbSet<Prize> Prizes => Set<Prize>();
    public DbSet<RefereeReport> RefereeReports => Set<RefereeReport>();
    public DbSet<JockeyInvitation> JockeyInvitations => Set<JockeyInvitation>();
    public DbSet<RaceAssignment> RaceAssignments => Set<RaceAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Cấu hình precision cho các cột decimal trong Bet
        modelBuilder.Entity<Bet>(b =>
        {
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.OddsMultiplier).HasPrecision(18, 4);
            b.Property(x => x.Payout).HasPrecision(18, 2);
        });
    }
}
