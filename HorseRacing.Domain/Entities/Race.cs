using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class Race
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RoundNumber { get; set; }
    public decimal Distance { get; set; } // in meters
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public RaceStatus Status { get; set; } = RaceStatus.Scheduled;
    public int MaxParticipants { get; set; }
    public string? TrackCondition { get; set; }
    public string? WeatherCondition { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Tournament Tournament { get; set; } = null!;
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public ICollection<RaceAssignment> RaceAssignments { get; set; } = new List<RaceAssignment>();
    public ICollection<RefereeReport> RefereeReports { get; set; } = new List<RefereeReport>();
}
