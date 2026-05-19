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

public record JockeyInvitationDto(
    int Id,
    int HorseId,
    string HorseName,
    int HorseOwnerId,
    string OwnerName,
    int JockeyId,
    string JockeyName,
    int? RaceId,
    string? RaceName,
    JockeyInvitationStatus Status,
    string? Message,
    string? ResponseMessage,
    DateTime InvitedAt,
    DateTime? RespondedAt
);
