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

public record RefereeReportDto(
    int Id,
    int RaceId,
    string RaceName,
    int RefereeUserId,
    string RefereeName,
    string Content,
    bool HasViolation,
    ViolationType? ViolationType,
    string? ViolationDescription,
    int? ViolatingRegistrationId,
    bool IsFinalized,
    DateTime CreatedAt
);
