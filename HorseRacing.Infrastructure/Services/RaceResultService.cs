using AutoMapper;
using HorseRacing.Application.DTOs.RaceResults;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RaceResultService : IRaceResultService
{
    private readonly IGenericRepository<RaceResult> _repo;
    private readonly IGenericRepository<Registration> _regRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RaceResultService(IGenericRepository<RaceResult> repo, IGenericRepository<Registration> regRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _regRepo = regRepo; _uow = uow; _mapper = mapper;
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
}
