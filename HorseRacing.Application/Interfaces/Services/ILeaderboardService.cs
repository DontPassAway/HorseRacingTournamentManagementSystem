using HorseRacing.Application.DTOs.Leaderboard;

namespace HorseRacing.Application.Interfaces.Services;

public interface ILeaderboardService
{
    Task<List<JockeyLeaderboardDto>> GetJockeyLeaderboardAsync(int top = 20);
    Task<List<HorseLeaderboardDto>> GetHorseLeaderboardAsync(int top = 20);
}
