namespace HorseRacing.Application.DTOs.RefereeReports;

// Input DTO để phạt vi phạm
public record CreatePenaltyDto(
    int RegistrationId,
    string PenaltyType,          // "Warning" | "Disqualified"
    string Reason
);

// Response DTO kết quả phạt
public class PenaltyResultDto
{
    public int ReportId { get; set; }
    public int RegistrationId { get; set; }
    public string HorseName { get; set; } = string.Empty;
    public string? JockeyName { get; set; }
    public string PenaltyType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public bool HorseDisqualified { get; set; }
    public DateTime AppliedAt { get; set; }
}
