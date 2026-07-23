using AutoMapper;
using HorseRacing.Application.DTOs.Races;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RaceService : IRaceService
{
    private readonly IGenericRepository<Race> _repo;
    private readonly IGenericRepository<Tournament> _tournamentRepo;
    private readonly IGenericRepository<Registration> _registrationRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RaceService(IGenericRepository<Race> repo, IGenericRepository<Tournament> tournamentRepo,
        IGenericRepository<Registration> registrationRepo, IGenericRepository<User> userRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _tournamentRepo = tournamentRepo;
        _registrationRepo = registrationRepo;
        _userRepo = userRepo;
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Race> BaseQuery() => _repo.Query()
        .Include(r => r.Tournament)
        .Include(r => r.Registrations);

    public async Task<RaceDto> CreateRaceAsync(CreateRaceDto dto)
    {
        _ = await _tournamentRepo.GetByIdAsync(dto.TournamentId)
            ?? throw new NotFoundException(nameof(Tournament), dto.TournamentId);

        var race = new Race
        {
            TournamentId = dto.TournamentId,
            Name = dto.Name,
            Description = dto.Description,
            RoundNumber = dto.RoundNumber,
            Distance = dto.Distance,
            ScheduledAt = dto.ScheduledAt,
            MaxParticipants = dto.MaxParticipants,
            TrackCondition = dto.TrackCondition,
            WeatherCondition = dto.WeatherCondition
        };
        await _repo.AddAsync(race);
        await _uow.SaveChangesAsync();
        return await GetRaceByIdAsync(race.Id);
    }

    public async Task<RaceDto> GetRaceByIdAsync(int id)
    {
        var race = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NotFoundException(nameof(Race), id);
        return _mapper.Map<RaceDto>(race);
    }

    public async Task<PagedResponse<RaceDto>> GetAllRacesAsync(int page, int pageSize, int? tournamentId = null)
    {
        var query = BaseQuery();
        if (tournamentId.HasValue) query = query.Where(r => r.TournamentId == tournamentId.Value);
        int total = await query.CountAsync();
        var items = await query.OrderBy(r => r.ScheduledAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<RaceDto>(_mapper.Map<List<RaceDto>>(items), page, pageSize, total);
    }

    public async Task<RaceDto> UpdateRaceAsync(int id, UpdateRaceDto dto)
    {
        var race = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Race), id);
        race.Name = dto.Name;
        race.Description = dto.Description;
        race.RoundNumber = dto.RoundNumber;
        race.Distance = dto.Distance;
        race.ScheduledAt = dto.ScheduledAt;
        race.MaxParticipants = dto.MaxParticipants;
        race.TrackCondition = dto.TrackCondition;
        race.WeatherCondition = dto.WeatherCondition;
        race.UpdatedAt = DateTime.UtcNow;
        _repo.Update(race);
        await _uow.SaveChangesAsync();
        return await GetRaceByIdAsync(id);
    }

    public async Task<RaceDto> UpdateStatusAsync(int id, RaceStatus status)
    {
        var race = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Race), id);
        race.Status = status;
        if (status == RaceStatus.InProgress) race.StartedAt = DateTime.UtcNow;
        if (status == RaceStatus.Completed) race.FinishedAt = DateTime.UtcNow;
        race.UpdatedAt = DateTime.UtcNow;
        _repo.Update(race);
        await _uow.SaveChangesAsync();
        return await GetRaceByIdAsync(id);
    }

    public async Task DeleteRaceAsync(int id)
    {
        var race = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Race), id);
        _repo.Remove(race);
        await _uow.SaveChangesAsync();
    }

    public async Task<HorseCheckResultDto> CheckHorseEligibilityAsync(int raceId, int refereeUserId, CheckHorseDto dto)
    {
        var race = await _repo.Query()
            .Include(r => r.Tournament)
            .FirstOrDefaultAsync(r => r.Id == raceId)
            ?? throw new NotFoundException(nameof(Race), raceId);

        var registration = await _registrationRepo.Query()
            .Include(r => r.Horse)
            .Include(r => r.Jockey).ThenInclude(j => j!.User)
            .FirstOrDefaultAsync(r => r.Id == dto.RegistrationId && r.RaceId == raceId)
            ?? throw new NotFoundException(nameof(Registration), dto.RegistrationId);

        var referee = await _userRepo.GetByIdAsync(refereeUserId)
            ?? throw new NotFoundException(nameof(User), refereeUserId);

        return new HorseCheckResultDto
        {
            RegistrationId = registration.Id,
            RaceId = race.Id,
            RaceName = race.Name,
            HorseId = registration.Horse.Id,
            HorseName = registration.Horse.Name,
            JockeyName = registration.Jockey?.User?.FullName,
            IsEligible = dto.IsEligible,
            CheckedWeight = dto.CheckedWeight,
            Notes = dto.Notes,
            CheckedAt = DateTime.UtcNow,
            CheckedByReferee = referee.FullName
        };
    }
}
