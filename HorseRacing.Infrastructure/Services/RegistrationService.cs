using AutoMapper;
using HorseRacing.Application.DTOs.Registrations;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IGenericRepository<Registration> _repo;
    private readonly IGenericRepository<Race> _raceRepo;
    private readonly IGenericRepository<Horse> _horseRepo;
    private readonly IGenericRepository<HorseOwner> _ownerRepo;
    private readonly IGenericRepository<JockeyProfile> _jockeyRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RegistrationService(
        IGenericRepository<Registration> repo,
        IGenericRepository<Race> raceRepo,
        IGenericRepository<Horse> horseRepo,
        IGenericRepository<HorseOwner> ownerRepo,
        IGenericRepository<JockeyProfile> jockeyRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _raceRepo = raceRepo; _horseRepo = horseRepo;
        _ownerRepo = ownerRepo; _jockeyRepo = jockeyRepo; _uow = uow; _mapper = mapper;
    }

    private IQueryable<Registration> BaseQuery() => _repo.Query()
        .Include(r => r.Race)
        .Include(r => r.Horse)
        .Include(r => r.HorseOwner).ThenInclude(o => o.User)
        .Include(r => r.Jockey).ThenInclude(j => j!.User);

    public async Task<RegistrationDto> RegisterHorseAsync(int ownerUserId, CreateRegistrationDto dto)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var horse = await _horseRepo.GetByIdAsync(dto.HorseId)
            ?? throw new NotFoundException(nameof(Horse), dto.HorseId);

        if (horse.HorseOwnerId != owner.Id)
            throw new ForbiddenException("You do not own this horse.");

        var race = await _raceRepo.GetByIdAsync(dto.RaceId)
            ?? throw new NotFoundException(nameof(Race), dto.RaceId);

        if (race.Status != RaceStatus.Scheduled)
            throw new BusinessException("Race is not open for registration.");

        var existing = await _repo.FirstOrDefaultAsync(r => r.RaceId == dto.RaceId && r.HorseId == dto.HorseId);
        if (existing != null)
            throw new BusinessException("Horse already registered for this race.");

        var registration = new Registration
        {
            RaceId = dto.RaceId,
            HorseId = dto.HorseId,
            HorseOwnerId = owner.Id,
            Status = RegistrationStatus.Pending,
            OwnerConfirmed = true
        };

        await _repo.AddAsync(registration);
        await _uow.SaveChangesAsync();
        return await GetRegistrationByIdAsync(registration.Id);
    }

    private async Task<RegistrationDto> GetRegistrationByIdAsync(int id)
    {
        var reg = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NotFoundException(nameof(Registration), id);
        return _mapper.Map<RegistrationDto>(reg);
    }

    public async Task<PagedResponse<RegistrationDto>> GetMyRegistrationsAsync(int ownerUserId, int page, int pageSize)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var query = BaseQuery().Where(r => r.HorseOwnerId == owner.Id);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<RegistrationDto>(_mapper.Map<List<RegistrationDto>>(items), page, pageSize, total);
    }

    public async Task<PagedResponse<RegistrationDto>> GetRegistrationsByRaceAsync(int raceId, int page, int pageSize)
    {
        var query = BaseQuery().Where(r => r.RaceId == raceId);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<RegistrationDto>(_mapper.Map<List<RegistrationDto>>(items), page, pageSize, total);
    }

    public async Task<RegistrationDto> ApproveRegistrationAsync(int id, ApproveRegistrationDto dto)
    {
        var reg = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Registration), id);
        reg.Status = RegistrationStatus.Approved;
        reg.LaneNumber = dto.LaneNumber;
        reg.ApprovedAt = DateTime.UtcNow;
        _repo.Update(reg);
        await _uow.SaveChangesAsync();
        return await GetRegistrationByIdAsync(id);
    }

    public async Task<RegistrationDto> RejectRegistrationAsync(int id, RejectRegistrationDto dto)
    {
        var reg = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Registration), id);
        reg.Status = RegistrationStatus.Rejected;
        reg.RejectionReason = dto.Reason;
        _repo.Update(reg);
        await _uow.SaveChangesAsync();
        return await GetRegistrationByIdAsync(id);
    }

    public async Task<RegistrationDto> ConfirmJockeyAsync(int id, int ownerUserId, ConfirmJockeyDto dto)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var reg = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Registration), id);

        if (reg.HorseOwnerId != owner.Id)
            throw new ForbiddenException("You do not own this registration.");

        var jockey = await _jockeyRepo.GetByIdAsync(dto.JockeyId)
            ?? throw new NotFoundException(nameof(JockeyProfile), dto.JockeyId);

        reg.JockeyId = jockey.Id;
        reg.JockeyConfirmed = true;
        reg.UpdatedAt = DateTime.UtcNow;
        _repo.Update(reg);
        await _uow.SaveChangesAsync();
        return await GetRegistrationByIdAsync(id);
    }

    public async Task WithdrawRegistrationAsync(int id, int ownerUserId)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);
        var reg = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Registration), id);
        if (reg.HorseOwnerId != owner.Id) throw new ForbiddenException();
        reg.Status = RegistrationStatus.Withdrawn;
        _repo.Update(reg);
        await _uow.SaveChangesAsync();
    }

    public async Task<PagedResponse<RegistrationDto>> GetMyRidesAsync(int jockeyUserId, int page, int pageSize)
    {
        var query = _repo.Query()
            .Include(r => r.Horse)
            .Include(r => r.Race)
            .Include(r => r.Jockey)
            .Where(r => r.Jockey != null && r.Jockey.UserId == jockeyUserId);

        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResponse<RegistrationDto>(_mapper.Map<List<RegistrationDto>>(items), page, pageSize, total);
    }

    public async Task<PagedResponse<RegistrationDto>> GetAllRegistrationsAsync(int page, int pageSize)
    {
        var query = _repo.Query(); 
        int total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.RegisteredAt)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        return new PagedResponse<RegistrationDto>(_mapper.Map<List<RegistrationDto>>(items), page, pageSize, total);
    }
}
