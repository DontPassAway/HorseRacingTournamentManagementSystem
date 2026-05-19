using AutoMapper;
using HorseRacing.Application.DTOs.Bets;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class BetService : IBetService
{
    private readonly IGenericRepository<Bet> _repo;
    private readonly IGenericRepository<Race> _raceRepo;
    private readonly IGenericRepository<Horse> _horseRepo;
    private readonly IGenericRepository<RaceResult> _resultRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public BetService(IGenericRepository<Bet> repo, IGenericRepository<Race> raceRepo,
        IGenericRepository<Horse> horseRepo, IGenericRepository<RaceResult> resultRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _raceRepo = raceRepo; _horseRepo = horseRepo;
        _resultRepo = resultRepo; _uow = uow; _mapper = mapper;
    }

    private IQueryable<Bet> BaseQuery() => _repo.Query()
        .Include(b => b.SpectatorUser)
        .Include(b => b.Race)
        .Include(b => b.PredictedHorse);

    public async Task<BetDto> PlaceBetAsync(int spectatorUserId, CreateBetDto dto)
    {
        var race = await _raceRepo.GetByIdAsync(dto.RaceId) ?? throw new NotFoundException(nameof(Race), dto.RaceId);
        if (race.Status != RaceStatus.Scheduled)
            throw new BusinessException("Bets can only be placed on scheduled races.");

        var horse = await _horseRepo.GetByIdAsync(dto.PredictedHorseId)
            ?? throw new NotFoundException(nameof(Horse), dto.PredictedHorseId);

        var existing = await _repo.FirstOrDefaultAsync(b => b.SpectatorUserId == spectatorUserId && b.RaceId == dto.RaceId);
        if (existing != null) throw new BusinessException("You already placed a bet on this race.");

        var bet = new Bet
        {
            SpectatorUserId = spectatorUserId,
            RaceId = dto.RaceId,
            PredictedHorseId = dto.PredictedHorseId,
            PredictedPosition = dto.PredictedPosition,
            Notes = dto.Notes
        };

        await _repo.AddAsync(bet);
        await _uow.SaveChangesAsync();

        var created = await BaseQuery().FirstOrDefaultAsync(b => b.Id == bet.Id);
        return _mapper.Map<BetDto>(created!);
    }

    public async Task<PagedResponse<BetDto>> GetMyBetsAsync(int spectatorUserId, int page, int pageSize)
    {
        var query = BaseQuery().Where(b => b.SpectatorUserId == spectatorUserId);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<BetDto>(_mapper.Map<List<BetDto>>(items), page, pageSize, total);
    }

    public async Task<PagedResponse<BetDto>> GetBetsByRaceAsync(int raceId, int page, int pageSize)
    {
        var query = BaseQuery().Where(b => b.RaceId == raceId);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<BetDto>(_mapper.Map<List<BetDto>>(items), page, pageSize, total);
    }

    public async Task ResolveBetsForRaceAsync(int raceId)
    {
        var bets = await _repo.FindAsync(b => b.RaceId == raceId && b.Status == BetStatus.Pending);
        var results = await _resultRepo.FindAsync(r => r.RaceId == raceId && r.IsConfirmed);

        foreach (var bet in bets)
        {
            var matchingResult = results.FirstOrDefault(r =>
                r.Registration.HorseId == bet.PredictedHorseId &&
                r.Position == bet.PredictedPosition &&
                !r.Disqualified);

            bet.Status = matchingResult != null ? BetStatus.Won : BetStatus.Lost;
            bet.ResolvedAt = DateTime.UtcNow;
            _repo.Update(bet);
        }
        await _uow.SaveChangesAsync();
    }
}
