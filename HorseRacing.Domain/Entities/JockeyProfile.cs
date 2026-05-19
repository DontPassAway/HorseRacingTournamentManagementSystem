namespace HorseRacing.Domain.Entities;

public class JockeyProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal Weight { get; set; }
    public int ExperienceYears { get; set; }
    public int TotalRaces { get; set; } = 0;
    public int TotalWins { get; set; } = 0;
    public string? Nationality { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<JockeyInvitation> ReceivedInvitations { get; set; } = new List<JockeyInvitation>();
}
