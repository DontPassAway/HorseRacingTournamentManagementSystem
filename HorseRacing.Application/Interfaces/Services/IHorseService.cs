using HorseRacing.Application.DTOs.Horses;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IHorseService
{
    Task<HorseDto> CreateHorseAsync(int ownerUserId, CreateHorseDto dto);
    Task<HorseDto> GetHorseByIdAsync(int id);
    Task<PagedResponse<HorseDto>> GetAllHorsesAsync(int page, int pageSize);
    Task<PagedResponse<HorseDto>> GetMyHorsesAsync(int ownerUserId, int page, int pageSize);
    Task<HorseDto> UpdateHorseAsync(int id, int ownerUserId, UpdateHorseDto dto);
    Task DeleteHorseAsync(int id, int ownerUserId);
}
