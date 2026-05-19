using HorseRacing.Application.DTOs.Prizes;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IPrizeService
{
    Task<PrizeDto> CreatePrizeAsync(CreatePrizeDto dto);
    Task<List<PrizeDto>> GetPrizesByTournamentAsync(int tournamentId);
    Task<PrizeDto> UpdatePrizeAsync(int id, UpdatePrizeDto dto);
    Task DeletePrizeAsync(int id);
}
