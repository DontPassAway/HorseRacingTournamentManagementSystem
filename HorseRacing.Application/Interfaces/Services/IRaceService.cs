using HorseRacing.Application.DTOs.Races;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IRaceService
{
    Task<RaceDto> CreateRaceAsync(CreateRaceDto dto);
    Task<RaceDto> GetRaceByIdAsync(int id);
    Task<PagedResponse<RaceDto>> GetAllRacesAsync(int page, int pageSize, int? tournamentId = null);
    Task<RaceDto> UpdateRaceAsync(int id, UpdateRaceDto dto);
    Task<RaceDto> UpdateStatusAsync(int id, RaceStatus status);
    Task DeleteRaceAsync(int id);
    Task<HorseCheckResultDto> CheckHorseEligibilityAsync(int raceId, int refereeUserId, CheckHorseDto dto);
}
