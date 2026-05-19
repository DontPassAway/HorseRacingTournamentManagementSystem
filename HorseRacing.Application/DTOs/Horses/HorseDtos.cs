using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Horses;

public record CreateHorseDto(
    string Name,
    string Breed,
    int Age,
    string Color,
    decimal Weight,
    string? MedicalHistory,
    string? ImageUrl
);

public record UpdateHorseDto(
    string Name,
    string Breed,
    int Age,
    string Color,
    decimal Weight,
    HorseStatus Status,
    string? MedicalHistory,
    string? ImageUrl
);

public record HorseDto(
    int Id,
    int HorseOwnerId,
    string OwnerName,
    string Name,
    string Breed,
    int Age,
    string Color,
    decimal Weight,
    HorseStatus Status,
    string? MedicalHistory,
    string? ImageUrl,
    int TotalRaces,
    int TotalWins,
    DateTime RegisteredAt
);
