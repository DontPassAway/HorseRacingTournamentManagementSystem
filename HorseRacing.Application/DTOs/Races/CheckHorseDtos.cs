namespace HorseRacing.Application.DTOs.Races;

// Input DTO cho trọng tài kiểm tra ngựa
public record CheckHorseDto(
    int RegistrationId,
    bool IsEligible,
    decimal? CheckedWeight,
    string? Notes
);

// Response DTO kết quả kiểm tra
public class HorseCheckResultDto
{
    public int RegistrationId { get; set; }
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public int HorseId { get; set; }
    public string HorseName { get; set; } = string.Empty;
    public string? JockeyName { get; set; }
    public bool IsEligible { get; set; }
    public decimal? CheckedWeight { get; set; }
    public string? Notes { get; set; }
    public DateTime CheckedAt { get; set; }
    public string CheckedByReferee { get; set; } = string.Empty;
}
