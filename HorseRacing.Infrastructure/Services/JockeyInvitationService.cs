using AutoMapper;
using HorseRacing.Application.DTOs.JockeyInvitations;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class JockeyInvitationService : IJockeyInvitationService
{
    private readonly IGenericRepository<JockeyInvitation> _repo;
    private readonly IGenericRepository<HorseOwner> _ownerRepo;
    private readonly IGenericRepository<JockeyProfile> _jockeyRepo;
    private readonly IGenericRepository<Horse> _horseRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public JockeyInvitationService(
        IGenericRepository<JockeyInvitation> repo,
        IGenericRepository<HorseOwner> ownerRepo,
        IGenericRepository<JockeyProfile> jockeyRepo,
        IGenericRepository<Horse> horseRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _ownerRepo = ownerRepo; _jockeyRepo = jockeyRepo;
        _horseRepo = horseRepo; _uow = uow; _mapper = mapper;
    }

    private IQueryable<JockeyInvitation> BaseQuery() => _repo.Query()
        .Include(i => i.Horse)
        .Include(i => i.HorseOwner).ThenInclude(o => o.User)
        .Include(i => i.Jockey).ThenInclude(j => j.User)
        .Include(i => i.Race);

    public async Task<JockeyInvitationDto> SendInvitationAsync(int ownerUserId, CreateJockeyInvitationDto dto)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var horse = await _horseRepo.GetByIdAsync(dto.HorseId)
            ?? throw new NotFoundException(nameof(Horse), dto.HorseId);

        if (horse.HorseOwnerId != owner.Id)
            throw new ForbiddenException("You do not own this horse.");

        var jockey = await _jockeyRepo.FirstOrDefaultAsync(j => j.UserId == dto.JockeyUserId)
            ?? throw new BusinessException("Jockey not found. User may not have a jockey profile.");

        var invitation = new JockeyInvitation
        {
            HorseId = dto.HorseId,
            HorseOwnerId = owner.Id,
            JockeyId = jockey.Id,
            RaceId = dto.RaceId,
            Message = dto.Message
        };

        await _repo.AddAsync(invitation);
        await _uow.SaveChangesAsync();

        var inv = await BaseQuery().FirstOrDefaultAsync(i => i.Id == invitation.Id);
        return _mapper.Map<JockeyInvitationDto>(inv!);
    }

    public async Task<PagedResponse<JockeyInvitationDto>> GetReceivedInvitationsAsync(int jockeyUserId, int page, int pageSize)
    {
        var jockey = await _jockeyRepo.FirstOrDefaultAsync(j => j.UserId == jockeyUserId)
            ?? throw new NotFoundException(nameof(JockeyProfile), jockeyUserId);

        var query = BaseQuery().Where(i => i.JockeyId == jockey.Id);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<JockeyInvitationDto>(_mapper.Map<List<JockeyInvitationDto>>(items), page, pageSize, total);
    }

    public async Task<PagedResponse<JockeyInvitationDto>> GetSentInvitationsAsync(int ownerUserId, int page, int pageSize)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var query = BaseQuery().Where(i => i.HorseOwnerId == owner.Id);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<JockeyInvitationDto>(_mapper.Map<List<JockeyInvitationDto>>(items), page, pageSize, total);
    }

    public async Task<JockeyInvitationDto> AcceptInvitationAsync(int invitationId, int jockeyUserId, RespondInvitationDto dto)
    {
        var jockey = await _jockeyRepo.FirstOrDefaultAsync(j => j.UserId == jockeyUserId)
            ?? throw new NotFoundException(nameof(JockeyProfile), jockeyUserId);

        var inv = await _repo.GetByIdAsync(invitationId) ?? throw new NotFoundException(nameof(JockeyInvitation), invitationId);
        if (inv.JockeyId != jockey.Id) throw new ForbiddenException();
        if (inv.Status != JockeyInvitationStatus.Pending) throw new BusinessException("Invitation already responded.");

        inv.Status = JockeyInvitationStatus.Accepted;
        inv.ResponseMessage = dto.ResponseMessage;
        inv.RespondedAt = DateTime.UtcNow;
        _repo.Update(inv);
        await _uow.SaveChangesAsync();

        var updated = await BaseQuery().FirstOrDefaultAsync(i => i.Id == invitationId);
        return _mapper.Map<JockeyInvitationDto>(updated!);
    }

    public async Task<JockeyInvitationDto> DeclineInvitationAsync(int invitationId, int jockeyUserId, RespondInvitationDto dto)
    {
        var jockey = await _jockeyRepo.FirstOrDefaultAsync(j => j.UserId == jockeyUserId)
            ?? throw new NotFoundException(nameof(JockeyProfile), jockeyUserId);

        var inv = await _repo.GetByIdAsync(invitationId) ?? throw new NotFoundException(nameof(JockeyInvitation), invitationId);
        if (inv.JockeyId != jockey.Id) throw new ForbiddenException();
        if (inv.Status != JockeyInvitationStatus.Pending) throw new BusinessException("Invitation already responded.");

        inv.Status = JockeyInvitationStatus.Declined;
        inv.ResponseMessage = dto.ResponseMessage;
        inv.RespondedAt = DateTime.UtcNow;
        _repo.Update(inv);
        await _uow.SaveChangesAsync();

        var updated = await BaseQuery().FirstOrDefaultAsync(i => i.Id == invitationId);
        return _mapper.Map<JockeyInvitationDto>(updated!);
    }
}
