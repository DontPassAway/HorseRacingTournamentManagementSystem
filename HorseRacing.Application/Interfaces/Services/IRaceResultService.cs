using HorseRacing.Application.DTOs.RaceResults;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IRaceResultService
{
    Task<RaceResultDto> CreateResultAsync(CreateRaceResultDto dto);
    Task<List<RaceResultDto>> GetResultsByRaceAsync(int raceId);
    Task<RaceResultDto> UpdateResultAsync(int id, UpdateRaceResultDto dto);
    Task<RaceResultDto> ConfirmResultAsync(int id, int confirmedByUserId);
}
