using HorseRacing.Application.DTOs.Bets;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IBetService
{
    Task<BetDto> PlaceBetAsync(int spectatorUserId, CreateBetDto dto);
    Task<PagedResponse<BetDto>> GetMyBetsAsync(int spectatorUserId, int page, int pageSize);
    Task<PagedResponse<BetDto>> GetBetsByRaceAsync(int raceId, int page, int pageSize);
    Task<PagedResponse<BetDto>> GetAllBetsAsync(int page, int pageSize);
    Task ResolveBetsForRaceAsync(int raceId);
}
