using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Tournaments;

public record CreateTournamentDto(
    string Name,
    string? Description,
    string Location,
    DateTime StartDate,
    DateTime EndDate,
    DateTime RegistrationDeadline,
    int MaxParticipants,
    string? Rules
);

public record UpdateTournamentDto(
    string Name,
    string? Description,
    string Location,
    DateTime StartDate,
    DateTime EndDate,
    DateTime RegistrationDeadline,
    TournamentStatus Status,
    int MaxParticipants,
    string? Rules
);

public class TournamentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public TournamentStatus Status { get; set; }
    public int MaxParticipants { get; set; }
    public string? Rules { get; set; }
    public int TotalRaces { get; set; }
    public DateTime CreatedAt { get; set; }
}
