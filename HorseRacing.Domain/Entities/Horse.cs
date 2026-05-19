using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class Horse
{
    public int Id { get; set; }
    public int HorseOwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Color { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public HorseStatus Status { get; set; } = HorseStatus.Active;
    public string? MedicalHistory { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalRaces { get; set; } = 0;
    public int TotalWins { get; set; } = 0;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public HorseOwner HorseOwner { get; set; } = null!;
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<JockeyInvitation> JockeyInvitations { get; set; } = new List<JockeyInvitation>();
}
