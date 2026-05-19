using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class Tournament
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
    public int MaxParticipants { get; set; }
    public string? Rules { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Race> Races { get; set; } = new List<Race>();
    public ICollection<Prize> Prizes { get; set; } = new List<Prize>();
}
