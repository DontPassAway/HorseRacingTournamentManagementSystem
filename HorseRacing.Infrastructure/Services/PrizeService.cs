using AutoMapper;
using HorseRacing.Application.DTOs.Prizes;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class PrizeService : IPrizeService
{
    private readonly IGenericRepository<Prize> _repo;
    private readonly IGenericRepository<Tournament> _tournamentRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PrizeService(IGenericRepository<Prize> repo, IGenericRepository<Tournament> tournamentRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _tournamentRepo = tournamentRepo; _uow = uow; _mapper = mapper;
    }

    public async Task<PrizeDto> CreatePrizeAsync(CreatePrizeDto dto)
    {
        _ = await _tournamentRepo.GetByIdAsync(dto.TournamentId)
            ?? throw new NotFoundException(nameof(Tournament), dto.TournamentId);
        var prize = new Prize { TournamentId = dto.TournamentId, Position = dto.Position, Amount = dto.Amount, Description = dto.Description };
        await _repo.AddAsync(prize);
        await _uow.SaveChangesAsync();
        var created = await _repo.Query().Include(p => p.Tournament).FirstOrDefaultAsync(p => p.Id == prize.Id);
        return _mapper.Map<PrizeDto>(created!);
    }

    public async Task<List<PrizeDto>> GetPrizesByTournamentAsync(int tournamentId)
    {
        var prizes = await _repo.Query().Include(p => p.Tournament)
            .Where(p => p.TournamentId == tournamentId).OrderBy(p => p.Position).ToListAsync();
        return _mapper.Map<List<PrizeDto>>(prizes);
    }

    public async Task<PrizeDto> UpdatePrizeAsync(int id, UpdatePrizeDto dto)
    {
        var prize = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Prize), id);
        prize.Position = dto.Position; prize.Amount = dto.Amount; prize.Description = dto.Description; prize.UpdatedAt = DateTime.UtcNow;
        _repo.Update(prize); await _uow.SaveChangesAsync();
        var updated = await _repo.Query().Include(p => p.Tournament).FirstOrDefaultAsync(p => p.Id == id);
        return _mapper.Map<PrizeDto>(updated!);
    }

    public async Task DeletePrizeAsync(int id)
    {
        var prize = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Prize), id);
        _repo.Remove(prize); await _uow.SaveChangesAsync();
    }
}
