using HorseRacing.Application.DTOs.Profiles;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IJockeyProfileService
{
    Task<PagedResponse<JockeyProfileDto>> GetAllJockeysAsync(int page, int pageSize);
    Task<JockeyProfileDto> GetJockeyByIdAsync(int id);
    Task<JockeyProfileDto> GetMyJockeyProfileAsync(int userId);
    Task<JockeyProfileDto> UpdateMyJockeyProfileAsync(int userId, UpdateJockeyProfileDto dto);
}

public interface IHorseOwnerProfileService
{
    Task<PagedResponse<HorseOwnerProfileDto>> GetAllOwnersAsync(int page, int pageSize);
    Task<HorseOwnerProfileDto> GetOwnerByIdAsync(int id);
    Task<HorseOwnerProfileDto> GetMyOwnerProfileAsync(int userId);
    Task<HorseOwnerProfileDto> UpdateMyOwnerProfileAsync(int userId, UpdateHorseOwnerProfileDto dto);
}