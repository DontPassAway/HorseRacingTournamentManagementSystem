using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public HorseOwner? HorseOwnerProfile { get; set; }
    public JockeyProfile? JockeyProfile { get; set; }
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public ICollection<RaceAssignment> RaceAssignments { get; set; } = new List<RaceAssignment>();
    public ICollection<RefereeReport> RefereeReports { get; set; } = new List<RefereeReport>();
}
