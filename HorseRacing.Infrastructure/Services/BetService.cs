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
        if (dto.Amount <= 0)
            throw new BusinessException("Bet amount must be greater than 0.");

        var race = await _raceRepo.GetByIdAsync(dto.RaceId) ?? throw new NotFoundException(nameof(Race), dto.RaceId);
        if (race.Status != RaceStatus.Scheduled)
            throw new BusinessException("Bets can only be placed on scheduled races.");

        _ = await _horseRepo.GetByIdAsync(dto.PredictedHorseId)
            ?? throw new NotFoundException(nameof(Horse), dto.PredictedHorseId);

        var existing = await _repo.FirstOrDefaultAsync(b => b.SpectatorUserId == spectatorUserId && b.RaceId == dto.RaceId);
        if (existing != null) throw new BusinessException("You already placed a bet on this race.");

        // Tính OddsMultiplier tại thời điểm đặt dựa trên pool tiền hiện tại
        // Odds = TotalPool / TotalAmountOnThisHorse (tối thiểu 1.1x)
        var existingBetsOnRace = await _repo.FindAsync(b => b.RaceId == dto.RaceId);
        var totalPool = existingBetsOnRace.Sum(b => b.Amount) + dto.Amount;
        var amountOnHorse = existingBetsOnRace
            .Where(b => b.PredictedHorseId == dto.PredictedHorseId)
            .Sum(b => b.Amount) + dto.Amount;
        var oddsMultiplier = amountOnHorse > 0
            ? Math.Max(1.1m, Math.Round(totalPool / amountOnHorse, 2))
            : 1.5m;

        var bet = new Bet
        {
            SpectatorUserId = spectatorUserId,
            RaceId = dto.RaceId,
            PredictedHorseId = dto.PredictedHorseId,
            PredictedPosition = dto.PredictedPosition,
            Amount = dto.Amount,
            OddsMultiplier = oddsMultiplier,
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

    public async Task<PagedResponse<BetDto>> GetAllBetsAsync(int page, int pageSize)
    {
        var query = BaseQuery();

        int total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.Id) 
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new PagedResponse<BetDto>(_mapper.Map<List<BetDto>>(items), page, pageSize, total);
    }

    public async Task ResolveBetsForRaceAsync(int raceId)
    {
        var bets = await _repo.FindAsync(b => b.RaceId == raceId && b.Status == BetStatus.Pending);
        var results = await _resultRepo.Query()
            .Include(r => r.Registration)
            .Where(r => r.RaceId == raceId && r.IsConfirmed)
            .ToListAsync();

        foreach (var bet in bets)
        {
            // Kiểm tra ngựa đoán thắng có về đúng vị trí không (và không bị disqualified)
            var matchingResult = results.FirstOrDefault(r =>
                r.Registration.HorseId == bet.PredictedHorseId &&
                r.Position == bet.PredictedPosition &&
                !r.Disqualified);

            if (matchingResult != null)
            {
                bet.Status = BetStatus.Won;
                // Tiền thắng = Số tiền đặt × Hệ số nhân lúc đặt
                bet.Payout = Math.Round(bet.Amount * bet.OddsMultiplier, 2);
            }
            else
            {
                bet.Status = BetStatus.Lost;
                bet.Payout = 0m;
            }

            bet.ResolvedAt = DateTime.UtcNow;
            _repo.Update(bet);
        }
        await _uow.SaveChangesAsync();
    }

    public async Task<BetOddsDto> GetOddsForRaceAsync(int raceId)
    {
        var race = await _raceRepo.GetByIdAsync(raceId)
            ?? throw new NotFoundException(nameof(Race), raceId);

        var bets = await BaseQuery()
            .Where(b => b.RaceId == raceId)
            .ToListAsync();

        var totalBets = bets.Count;
        var totalPool = bets.Sum(b => b.Amount);

        var grouped = bets
            .GroupBy(b => new { b.PredictedHorseId, HorseName = b.PredictedHorse.Name })
            .Select(g =>
            {
                var horsePool = g.Sum(b => b.Amount);
                var odds = horsePool > 0
                    ? Math.Max(1.1m, Math.Round(totalPool / horsePool, 2))
                    : 0m;
                return new HorseOddsDto
                {
                    HorseId = g.Key.PredictedHorseId,
                    HorseName = g.Key.HorseName,
                    BetCount = g.Count(),
                    TotalAmountBet = horsePool,
                    Percentage = totalPool > 0 ? Math.Round(horsePool / totalPool * 100, 2) : 0,
                    OddsMultiplier = odds
                };
            })
            .OrderByDescending(o => o.TotalAmountBet)
            .ToList();

        return new BetOddsDto
        {
            RaceId = raceId,
            RaceName = race.Name,
            TotalBets = totalBets,
            TotalPoolAmount = totalPool,
            Odds = grouped
        };
    }

    public async Task<BetSummaryDto> GetMySummaryAsync(int spectatorUserId)
    {
        var betsEnum = await _repo.FindAsync(b => b.SpectatorUserId == spectatorUserId);
        var bets = betsEnum.ToList();

        var totalWon    = bets.Count(b => b.Status == BetStatus.Won);
        var totalLost   = bets.Count(b => b.Status == BetStatus.Lost);
        var totalPend   = bets.Count(b => b.Status == BetStatus.Pending);
        var totalAmount = bets.Sum(b => b.Amount);
        var totalPayout = bets.Sum(b => b.Payout ?? 0m);
        var winRate     = bets.Count > 0
            ? Math.Round((decimal)totalWon / bets.Count * 100, 2)
            : 0m;

        return new BetSummaryDto
        {
            TotalBets      = bets.Count,
            TotalWon       = totalWon,
            TotalLost      = totalLost,
            TotalPending   = totalPend,
            TotalAmountBet = totalAmount,
            TotalPayout    = totalPayout,
            NetProfit      = totalPayout - totalAmount,
            WinRate        = winRate
        };
    }

    public async Task<List<BetLeaderboardEntryDto>> GetBettingLeaderboardAsync(int top = 10)
    {
        var allBets = await BaseQuery()
            .Where(b => b.Status == BetStatus.Won || b.Status == BetStatus.Lost)
            .ToListAsync();

        var grouped = allBets
            .GroupBy(b => new { b.SpectatorUserId, SpectatorName = b.SpectatorUser.FullName })
            .Select(g => new
            {
                g.Key.SpectatorUserId,
                g.Key.SpectatorName,
                TotalBets      = g.Count(),
                TotalWins      = g.Count(b => b.Status == BetStatus.Won),
                TotalAmountBet = g.Sum(b => b.Amount),
                TotalPayout    = g.Sum(b => b.Payout ?? 0m)
            })
            .OrderByDescending(x => x.TotalPayout - x.TotalAmountBet)
            .Take(top)
            .ToList();

        return grouped.Select((x, i) => new BetLeaderboardEntryDto
        {
            Rank            = i + 1,
            SpectatorUserId = x.SpectatorUserId,
            SpectatorName   = x.SpectatorName,
            TotalBets       = x.TotalBets,
            TotalWins       = x.TotalWins,
            TotalAmountBet  = x.TotalAmountBet,
            TotalPayout     = x.TotalPayout,
            NetProfit       = x.TotalPayout - x.TotalAmountBet
        }).ToList();
    }
}
