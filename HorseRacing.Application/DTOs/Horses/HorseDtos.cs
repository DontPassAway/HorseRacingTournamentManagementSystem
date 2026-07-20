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

public class HorseDto
{
    public int Id { get; set; }
    public int HorseOwnerId { get; set; }
    public string OwnerName { get; set; }
    public string Name { get; set; }
    public string Breed { get; set; }
    public int Age { get; set; }
    public string Color { get; set; }
    public decimal Weight { get; set; }
    public HorseStatus Status { get; set; }
    public string? MedicalHistory { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalRaces { get; set; }
    public int TotalWins { get; set; }
    public DateTime RegisteredAt { get; set; }
}
