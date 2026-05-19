namespace HorseRacing.Application.DTOs.RaceResults;

public record CreateRaceResultDto(
    int RaceId,
    int RegistrationId,
    int Position,
    TimeSpan? FinishTime,
    bool Disqualified,
    string? DisqualificationReason
);

public record UpdateRaceResultDto(
    int Position,
    TimeSpan? FinishTime,
    bool Disqualified,
    string? DisqualificationReason
);

public record RaceResultDto(
    int Id,
    int RaceId,
    string RaceName,
    int RegistrationId,
    int HorseId,
    string HorseName,
    string? JockeyName,
    int Position,
    TimeSpan? FinishTime,
    bool Disqualified,
    string? DisqualificationReason,
    decimal? PrizeMoney,
    bool IsConfirmed
);
