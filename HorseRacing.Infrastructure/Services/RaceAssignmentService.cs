using AutoMapper;
using HorseRacing.Application.DTOs.RaceAssignments;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RaceAssignmentService : IRaceAssignmentService
{
    private readonly IGenericRepository<RaceAssignment> _repo;
    private readonly IGenericRepository<Race> _raceRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RaceAssignmentService(IGenericRepository<RaceAssignment> repo, IGenericRepository<Race> raceRepo,
        IGenericRepository<User> userRepo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _raceRepo = raceRepo; _userRepo = userRepo; _uow = uow; _mapper = mapper;
    }

    private IQueryable<RaceAssignment> BaseQuery() => _repo.Query()
        .Include(a => a.Race).Include(a => a.RefereeUser);

    public async Task<RaceAssignmentDto> AssignRefereeAsync(CreateRaceAssignmentDto dto)
    {
        _ = await _raceRepo.GetByIdAsync(dto.RaceId) ?? throw new NotFoundException(nameof(Race), dto.RaceId);
        var referee = await _userRepo.GetByIdAsync(dto.RefereeUserId) ?? throw new NotFoundException(nameof(User), dto.RefereeUserId);
        if (referee.Role != Domain.Enums.UserRole.Referee) throw new BusinessException("User is not a referee.");

        var existing = await _repo.FirstOrDefaultAsync(a => a.RaceId == dto.RaceId && a.RefereeUserId == dto.RefereeUserId);
        if (existing != null) throw new BusinessException("Referee already assigned to this race.");

        var assignment = new RaceAssignment { RaceId = dto.RaceId, RefereeUserId = dto.RefereeUserId, Notes = dto.Notes };
        await _repo.AddAsync(assignment); await _uow.SaveChangesAsync();
        var created = await BaseQuery().FirstOrDefaultAsync(a => a.Id == assignment.Id);
        return _mapper.Map<RaceAssignmentDto>(created!);
    }

    public async Task<PagedResponse<RaceAssignmentDto>> GetMyAssignmentsAsync(int refereeUserId, int page, int pageSize)
    {
        var query = BaseQuery().Where(a => a.RefereeUserId == refereeUserId);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<RaceAssignmentDto>(_mapper.Map<List<RaceAssignmentDto>>(items), page, pageSize, total);
    }

    public async Task DeleteAssignmentAsync(int id)
    {
        var a = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(RaceAssignment), id);
        _repo.Remove(a); await _uow.SaveChangesAsync();
    }
}
