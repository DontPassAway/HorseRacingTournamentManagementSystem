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

public record TournamentDto(
    int Id,
    string Name,
    string? Description,
    string Location,
    DateTime StartDate,
    DateTime EndDate,
    DateTime RegistrationDeadline,
    TournamentStatus Status,
    int MaxParticipants,
    string? Rules,
    int TotalRaces,
    DateTime CreatedAt
);
