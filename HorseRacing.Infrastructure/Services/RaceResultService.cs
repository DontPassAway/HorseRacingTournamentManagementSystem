using AutoMapper;
using HorseRacing.Application.DTOs.RaceResults;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RaceResultService : IRaceResultService
{
    private readonly IGenericRepository<RaceResult> _repo;
    private readonly IGenericRepository<Registration> _regRepo;
    private readonly IGenericRepository<Race> _raceRepo;
    private readonly IGenericRepository<Horse> _horseRepo;
    private readonly IGenericRepository<JockeyProfile> _jockeyRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RaceResultService(
        IGenericRepository<RaceResult> repo,
        IGenericRepository<Registration> regRepo,
        IGenericRepository<Race> raceRepo,
        IGenericRepository<Horse> horseRepo,
        IGenericRepository<JockeyProfile> jockeyRepo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _repo = repo;
        _regRepo = regRepo;
        _raceRepo = raceRepo;
        _horseRepo = horseRepo;
        _jockeyRepo = jockeyRepo;
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<RaceResult> BaseQuery() => _repo.Query()
        .Include(r => r.Race)
        .Include(r => r.Registration).ThenInclude(reg => reg.Horse)
        .Include(r => r.Registration).ThenInclude(reg => reg.Jockey).ThenInclude(j => j!.User);

    public async Task<RaceResultDto> CreateResultAsync(CreateRaceResultDto dto)
    {
        _ = await _regRepo.GetByIdAsync(dto.RegistrationId)
            ?? throw new NotFoundException(nameof(Registration), dto.RegistrationId);

        var result = new RaceResult
        {
            RaceId = dto.RaceId,
            RegistrationId = dto.RegistrationId,
            Position = dto.Position,
            FinishTime = dto.FinishTime,
            Disqualified = dto.Disqualified,
            DisqualificationReason = dto.DisqualificationReason
        };

        await _repo.AddAsync(result);
        await _uow.SaveChangesAsync();

        var created = await BaseQuery().FirstOrDefaultAsync(r => r.Id == result.Id);
        return _mapper.Map<RaceResultDto>(created!);
    }

    public async Task<List<RaceResultDto>> GetResultsByRaceAsync(int raceId)
    {
        var results = await BaseQuery()
            .Where(r => r.RaceId == raceId)
            .OrderBy(r => r.Position)
            .ToListAsync();
        return _mapper.Map<List<RaceResultDto>>(results);
    }

    public async Task<RaceResultDto> UpdateResultAsync(int id, UpdateRaceResultDto dto)
    {
        var result = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(RaceResult), id);
        result.Position = dto.Position;
        result.FinishTime = dto.FinishTime;
        result.Disqualified = dto.Disqualified;
        result.DisqualificationReason = dto.DisqualificationReason;
        result.UpdatedAt = DateTime.UtcNow;
        _repo.Update(result);
        await _uow.SaveChangesAsync();

        var updated = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id);
        return _mapper.Map<RaceResultDto>(updated!);
    }

    public async Task<RaceResultDto> ConfirmResultAsync(int id, int confirmedByUserId)
    {
        var result = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(RaceResult), id);
        result.IsConfirmed = true;
        result.ConfirmedByUserId = confirmedByUserId;
        result.UpdatedAt = DateTime.UtcNow;
        _repo.Update(result);
        await _uow.SaveChangesAsync();

        var updated = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id);
        return _mapper.Map<RaceResultDto>(updated!);
    }

    public async Task<List<RaceResultDto>> SimulateRaceAsync(int raceId, int adminUserId)
    {
        var race = await _raceRepo.GetByIdAsync(raceId)
            ?? throw new NotFoundException(nameof(Race), raceId);

        if (race.Status == RaceStatus.Completed)
            throw new BusinessException("Race has already been completed.");

        // Get all approved registrations with confirmed jockeys
        var registrations = await _regRepo.Query()
            .Include(r => r.Horse)
            .Include(r => r.Jockey).ThenInclude(j => j!.User)
            .Where(r => r.RaceId == raceId && r.Status == RegistrationStatus.Approved)
            .ToListAsync();

        if (registrations.Count == 0)
            throw new BusinessException("No approved registrations found for this race. Cannot simulate.");

        // Delete any existing (unconfirmed) results for this race
        var existingResults = await _repo.FindAsync(r => r.RaceId == raceId && !r.IsConfirmed);
        foreach (var er in existingResults)
            _repo.Remove(er);

        // Shuffle registrations randomly to determine finishing order
        var rng = new Random();
        var shuffled = registrations.OrderBy(_ => rng.Next()).ToList();

        // Base finish time: realistic for a horse race (roughly 60-120 seconds)
        // Each subsequent position adds a small random gap (0.3–3.0 seconds)
        var baseTime = TimeSpan.FromSeconds(60 + rng.NextDouble() * 60);
        var newResults = new List<RaceResult>();

        for (int i = 0; i < shuffled.Count; i++)
        {
            var gapSeconds = i == 0 ? 0 : (0.3 + rng.NextDouble() * 2.7) * i;
            var finishTime = baseTime + TimeSpan.FromSeconds(gapSeconds);

            var result = new RaceResult
            {
                RaceId = raceId,
                RegistrationId = shuffled[i].Id,
                Position = i + 1,
                FinishTime = finishTime,
                Disqualified = false,
                IsConfirmed = true,
                ConfirmedByUserId = adminUserId
            };
            await _repo.AddAsync(result);
            newResults.Add(result);
        }

        // Update race status to Completed
        race.Status = RaceStatus.Completed;
        race.FinishedAt = DateTime.UtcNow;
        if (race.StartedAt == null) race.StartedAt = DateTime.UtcNow - TimeSpan.FromMinutes(5);
        race.UpdatedAt = DateTime.UtcNow;
        _raceRepo.Update(race);

        // Update winner horse stats
        var winnerReg = shuffled[0];
        var winnerHorse = winnerReg.Horse;
        winnerHorse.TotalRaces += 1;
        winnerHorse.TotalWins += 1;
        winnerHorse.UpdatedAt = DateTime.UtcNow;
        _horseRepo.Update(winnerHorse);

        // Update all other horses' TotalRaces
        for (int i = 1; i < shuffled.Count; i++)
        {
            var h = shuffled[i].Horse;
            h.TotalRaces += 1;
            h.UpdatedAt = DateTime.UtcNow;
            _horseRepo.Update(h);
        }

        // Update winner jockey stats
        if (winnerReg.Jockey != null)
        {
            winnerReg.Jockey.TotalRaces += 1;
            winnerReg.Jockey.TotalWins += 1;
            _jockeyRepo.Update(winnerReg.Jockey);
        }
        for (int i = 1; i < shuffled.Count; i++)
        {
            var jockey = shuffled[i].Jockey;
            if (jockey != null)
            {
                jockey.TotalRaces += 1;
                _jockeyRepo.Update(jockey);
            }
        }

        await _uow.SaveChangesAsync();

        // Return full results ordered by position
        var resultIds = newResults.Select(r => r.Id).ToList();
        var fullResults = await BaseQuery()
            .Where(r => resultIds.Contains(r.Id))
            .OrderBy(r => r.Position)
            .ToListAsync();

        return _mapper.Map<List<RaceResultDto>>(fullResults);
    }
}
