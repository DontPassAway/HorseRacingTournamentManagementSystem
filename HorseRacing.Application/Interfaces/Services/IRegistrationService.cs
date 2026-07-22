using HorseRacing.Application.DTOs.Registrations;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IRegistrationService
{
    Task<RegistrationDto> RegisterHorseAsync(int ownerUserId, CreateRegistrationDto dto);
    Task<PagedResponse<RegistrationDto>> GetMyRegistrationsAsync(int ownerUserId, int page, int pageSize);
    Task<PagedResponse<RegistrationDto>> GetRegistrationsByRaceAsync(int raceId, int page, int pageSize);
    Task<RegistrationDto> ApproveRegistrationAsync(int id, ApproveRegistrationDto dto);
    Task<RegistrationDto> RejectRegistrationAsync(int id, RejectRegistrationDto dto);
    Task<RegistrationDto> ConfirmJockeyAsync(int id, int ownerUserId, ConfirmJockeyDto dto);
    Task WithdrawRegistrationAsync(int id, int ownerUserId);

    Task<PagedResponse<RegistrationDto>> GetMyRidesAsync(int jockeyUserId, int page, int pageSize);

    Task<PagedResponse<RegistrationDto>> GetAllRegistrationsAsync(int page, int pageSize);
}
