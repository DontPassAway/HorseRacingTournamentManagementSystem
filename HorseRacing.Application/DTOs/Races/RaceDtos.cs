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

public record RaceDto(
    int Id,
    int TournamentId,
    string TournamentName,
    string Name,
    string? Description,
    int RoundNumber,
    decimal Distance,
    DateTime ScheduledAt,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    RaceStatus Status,
    int MaxParticipants,
    string? TrackCondition,
    string? WeatherCondition,
    int TotalRegistrations
);
