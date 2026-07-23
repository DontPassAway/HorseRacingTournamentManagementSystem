using HorseRacing.Application.DTOs.Leaderboard;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IGenericRepository<JockeyProfile> _jockeyRepo;
    private readonly IGenericRepository<Horse> _horseRepo;

    public LeaderboardService(
        IGenericRepository<JockeyProfile> jockeyRepo,
        IGenericRepository<Horse> horseRepo)
    {
        _jockeyRepo = jockeyRepo;
        _horseRepo = horseRepo;
    }

    public async Task<List<JockeyLeaderboardDto>> GetJockeyLeaderboardAsync(int top = 20)
    {
        var jockeys = await _jockeyRepo.Query()
            .Include(j => j.User)
            .Where(j => j.TotalRaces > 0)
            .OrderByDescending(j => j.TotalWins)
            .ThenByDescending(j => j.TotalRaces)
            .Take(top)
            .ToListAsync();

        return jockeys.Select((j, index) => new JockeyLeaderboardDto
        {
            Rank = index + 1,
            JockeyId = j.Id,
            UserId = j.UserId,
            JockeyName = j.User.FullName,
            TotalRaces = j.TotalRaces,
            TotalWins = j.TotalWins,
            WinRate = j.TotalRaces > 0
                ? Math.Round((decimal)j.TotalWins / j.TotalRaces * 100, 2)
                : 0
        }).ToList();
    }

    public async Task<List<HorseLeaderboardDto>> GetHorseLeaderboardAsync(int top = 20)
    {
        var horses = await _horseRepo.Query()
            .Include(h => h.HorseOwner).ThenInclude(o => o.User)
            .Where(h => h.TotalRaces > 0)
            .OrderByDescending(h => h.TotalWins)
            .ThenByDescending(h => h.TotalRaces)
            .Take(top)
            .ToListAsync();

        return horses.Select((h, index) => new HorseLeaderboardDto
        {
            Rank = index + 1,
            HorseId = h.Id,
            HorseName = h.Name,
            OwnerName = h.HorseOwner.User.FullName,
            Breed = h.Breed,
            TotalRaces = h.TotalRaces,
            TotalWins = h.TotalWins,
            WinRate = h.TotalRaces > 0
                ? Math.Round((decimal)h.TotalWins / h.TotalRaces * 100, 2)
                : 0
        }).ToList();
    }
}
