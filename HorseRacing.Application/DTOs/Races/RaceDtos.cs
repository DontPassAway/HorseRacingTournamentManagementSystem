using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Races;

public record CreateRaceDto(
    int TournamentId,
    string Name,
    string? Description,
    int RoundNumber,
    decimal Distance,
    DateTime ScheduledAt,
    int MaxParticipants,
    string? TrackCondition,
    string? WeatherCondition
);

public record UpdateRaceDto(
    string Name,
    string? Description,
    int RoundNumber,
    decimal Distance,
    DateTime ScheduledAt,
    int MaxParticipants,
    string? TrackCondition,
    string? WeatherCondition
);

public class RaceDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string TournamentName { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int RoundNumber { get; set; }
    public decimal Distance { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public RaceStatus Status { get; set; }
    public int MaxParticipants { get; set; }
    public string? TrackCondition { get; set; }
    public string? WeatherCondition { get; set; }
    public int TotalRegistrations { get; set; }
}
