using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Registrations;

public record CreateRegistrationDto(
    int RaceId,
    int HorseId
);

public class RegistrationDto
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public string RaceName { get; set; }
    public int HorseId { get; set; }
    public string HorseName { get; set; }
    public int HorseOwnerId { get; set; }
    public string OwnerName { get; set; }
    public int? JockeyId { get; set; }
    public string? JockeyName { get; set; }
    public int LaneNumber { get; set; }
    public RegistrationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public bool JockeyConfirmed { get; set; }
    public bool OwnerConfirmed { get; set; }
    public DateTime RegisteredAt { get; set; }
}

public record ConfirmJockeyDto(int JockeyId);

public record ApproveRegistrationDto(int LaneNumber);

public record RejectRegistrationDto(string Reason);
