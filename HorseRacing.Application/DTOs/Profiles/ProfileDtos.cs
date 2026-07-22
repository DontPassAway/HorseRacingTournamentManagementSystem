namespace HorseRacing.Application.DTOs.Profiles;

// --- JOCKEY DTOs ---
public class JockeyProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int ExperienceYears { get; set; }
    public decimal Weight { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Nationality { get; set; }
    public int TotalRaces { get; set; }
    public int TotalWins { get; set; }
}

public record UpdateJockeyProfileDto(
    int ExperienceYears,
    decimal Weight,
    string? LicenseNumber,
    string? Nationality
);

// --- HORSE OWNER DTOs ---
public class HorseOwnerProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime RegisteredAt { get; set; }
}

public record UpdateHorseOwnerProfileDto(
    string? Address,
    string? LicenseNumber,
    DateTime? DateOfBirth
);