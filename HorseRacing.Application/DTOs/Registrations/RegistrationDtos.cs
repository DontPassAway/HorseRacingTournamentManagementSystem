using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Registrations;

public record CreateRegistrationDto(
    int RaceId,
    int HorseId
);

public record RegistrationDto(
    int Id,
    int RaceId,
    string RaceName,
    int HorseId,
    string HorseName,
    int HorseOwnerId,
    string OwnerName,
    int? JockeyId,
    string? JockeyName,
    int LaneNumber,
    RegistrationStatus Status,
    string? RejectionReason,
    bool JockeyConfirmed,
    bool OwnerConfirmed,
    DateTime RegisteredAt
);

public record ConfirmJockeyDto(int JockeyId);

public record ApproveRegistrationDto(int LaneNumber);

public record RejectRegistrationDto(string Reason);
