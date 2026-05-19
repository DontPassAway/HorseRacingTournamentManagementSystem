namespace HorseRacing.Application.DTOs.RaceAssignments;

public record CreateRaceAssignmentDto(
    int RaceId,
    int RefereeUserId,
    string? Notes
);

public record RaceAssignmentDto(
    int Id,
    int RaceId,
    string RaceName,
    int RefereeUserId,
    string RefereeName,
    string? Notes,
    DateTime AssignedAt
);
