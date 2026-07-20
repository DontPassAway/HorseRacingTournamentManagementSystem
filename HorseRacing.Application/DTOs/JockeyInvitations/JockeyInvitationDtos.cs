using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.JockeyInvitations;

public record CreateJockeyInvitationDto(
    int HorseId,
    int JockeyUserId, // the User.Id of the jockey
    int? RaceId,
    string? Message
);

public record RespondInvitationDto(
    string? ResponseMessage
);

public class JockeyInvitationDto
{
    public int Id { get; set; }
    public int HorseId { get; set; }
    public string HorseName { get; set; }
    public int HorseOwnerId { get; set; }
    public string OwnerName { get; set; }
    public int JockeyId { get; set; }
    public string JockeyName { get; set; }
    public int? RaceId { get; set; }
    public string? RaceName { get; set; }
    public JockeyInvitationStatus Status { get; set; }
    public string? Message { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime InvitedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
