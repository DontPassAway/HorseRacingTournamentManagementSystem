using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.RefereeReports;

public record CreateRefereeReportDto(
    int RaceId,
    string Content,
    bool HasViolation,
    ViolationType? ViolationType,
    string? ViolationDescription,
    int? ViolatingRegistrationId
);

public record UpdateRefereeReportDto(
    string Content,
    bool HasViolation,
    ViolationType? ViolationType,
    string? ViolationDescription,
    int? ViolatingRegistrationId,
    bool IsFinalized
);

public class RefereeReportDto
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public string RaceName { get; set; }
    public int RefereeUserId { get; set; }
    public string RefereeName { get; set; }
    public string Content { get; set; }
    public bool HasViolation { get; set; }
    public ViolationType? ViolationType { get; set; }
    public string? ViolationDescription { get; set; }
    public int? ViolatingRegistrationId { get; set; }
    public bool IsFinalized { get; set; }
    public DateTime CreatedAt { get; set; }
}
