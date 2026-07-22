using HorseRacing.Application.DTOs.JockeyInvitations;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IJockeyInvitationService
{
    Task<JockeyInvitationDto> SendInvitationAsync(int ownerUserId, CreateJockeyInvitationDto dto);
    Task<PagedResponse<JockeyInvitationDto>> GetReceivedInvitationsAsync(int jockeyUserId, int page, int pageSize);
    Task<PagedResponse<JockeyInvitationDto>> GetSentInvitationsAsync(int ownerUserId, int page, int pageSize);
    Task<JockeyInvitationDto> AcceptInvitationAsync(int invitationId, int jockeyUserId, RespondInvitationDto dto);
    Task<JockeyInvitationDto> DeclineInvitationAsync(int invitationId, int jockeyUserId, RespondInvitationDto dto);

}
