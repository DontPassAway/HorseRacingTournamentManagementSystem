using AutoMapper;
using HorseRacing.Application.DTOs.Races;
using HorseRacing.Application.DTOs.Tournaments;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class TournamentService : ITournamentService
{
    private readonly IGenericRepository<Tournament> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TournamentService(IGenericRepository<Tournament> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<TournamentDto> CreateTournamentAsync(CreateTournamentDto dto)
    {
        var tournament = new Tournament
        {
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RegistrationDeadline = dto.RegistrationDeadline,
            MaxParticipants = dto.MaxParticipants,
            Rules = dto.Rules,
            Status = TournamentStatus.Draft
        };
        await _repo.AddAsync(tournament);
        await _uow.SaveChangesAsync();
        return await GetTournamentByIdAsync(tournament.Id);
    }

    public async Task<TournamentDto> GetTournamentByIdAsync(int id)
    {
        var t = await _repo.Query()
            .Include(t => t.Races)
            .Include(t => t.Prizes)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException(nameof(Tournament), id);
        return _mapper.Map<TournamentDto>(t);
    }

    public async Task<PagedResponse<TournamentDto>> GetAllTournamentsAsync(int page, int pageSize)
    {
        var query = _repo.Query().Include(t => t.Races).Include(t => t.Prizes);
        int total = await query.CountAsync();
        var items = await query.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<TournamentDto>(_mapper.Map<List<TournamentDto>>(items), page, pageSize, total);
    }

    public async Task<TournamentDto> UpdateTournamentAsync(int id, UpdateTournamentDto dto)
    {
        var t = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Tournament), id);
        t.Name = dto.Name;
        t.Description = dto.Description;
        t.Location = dto.Location;
        t.StartDate = dto.StartDate;
        t.EndDate = dto.EndDate;
        t.RegistrationDeadline = dto.RegistrationDeadline;
        t.Status = dto.Status;
        t.MaxParticipants = dto.MaxParticipants;
        t.Rules = dto.Rules;
        t.UpdatedAt = DateTime.UtcNow;
        _repo.Update(t);
        await _uow.SaveChangesAsync();
        return await GetTournamentByIdAsync(id);
    }

    public async Task<TournamentDto> UpdateStatusAsync(int id, TournamentStatus status)
    {
        var t = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Tournament), id);
        t.Status = status;
        t.UpdatedAt = DateTime.UtcNow;
        _repo.Update(t);
        await _uow.SaveChangesAsync();
        return await GetTournamentByIdAsync(id);
    }

    public async Task DeleteTournamentAsync(int id)
    {
        var t = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Tournament), id);
        _repo.Remove(t);
        await _uow.SaveChangesAsync();
    }

    public async Task<List<RaceDto>> GetRacesByTournamentAsync(int tournamentId)
    {
        var tournament = await _repo.Query()
            .Include(t => t.Races).ThenInclude(r => r.Registrations)
            .FirstOrDefaultAsync(t => t.Id == tournamentId)
            ?? throw new NotFoundException(nameof(Tournament), tournamentId);

        var races = tournament.Races.OrderBy(r => r.RoundNumber).ThenBy(r => r.ScheduledAt).ToList();
        // Manually assign Tournament navigation for mapping
        foreach (var race in races) race.Tournament = tournament;
        return _mapper.Map<List<RaceDto>>(races);
    }
}
