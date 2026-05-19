using AutoMapper;
using HorseRacing.Application.DTOs.Horses;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class HorseService : IHorseService
{
    private readonly IGenericRepository<Horse> _horseRepo;
    private readonly IGenericRepository<HorseOwner> _ownerRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public HorseService(
        IGenericRepository<Horse> horseRepo,
        IGenericRepository<HorseOwner> ownerRepo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _horseRepo = horseRepo;
        _ownerRepo = ownerRepo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<HorseDto> CreateHorseAsync(int ownerUserId, CreateHorseDto dto)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var horse = new Horse
        {
            HorseOwnerId = owner.Id,
            Name = dto.Name,
            Breed = dto.Breed,
            Age = dto.Age,
            Color = dto.Color,
            Weight = dto.Weight,
            MedicalHistory = dto.MedicalHistory,
            ImageUrl = dto.ImageUrl
        };

        await _horseRepo.AddAsync(horse);
        await _uow.SaveChangesAsync();

        return await GetHorseByIdAsync(horse.Id);
    }

    public async Task<HorseDto> GetHorseByIdAsync(int id)
    {
        var horse = await _horseRepo.Query()
            .Include(h => h.HorseOwner).ThenInclude(o => o.User)
            .FirstOrDefaultAsync(h => h.Id == id)
            ?? throw new NotFoundException(nameof(Horse), id);

        return _mapper.Map<HorseDto>(horse);
    }

    public async Task<PagedResponse<HorseDto>> GetAllHorsesAsync(int page, int pageSize)
    {
        var query = _horseRepo.Query().Include(h => h.HorseOwner).ThenInclude(o => o.User);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<HorseDto>(_mapper.Map<List<HorseDto>>(items), page, pageSize, total);
    }

    public async Task<PagedResponse<HorseDto>> GetMyHorsesAsync(int ownerUserId, int page, int pageSize)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var query = _horseRepo.Query()
            .Include(h => h.HorseOwner).ThenInclude(o => o.User)
            .Where(h => h.HorseOwnerId == owner.Id);

        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<HorseDto>(_mapper.Map<List<HorseDto>>(items), page, pageSize, total);
    }

    public async Task<HorseDto> UpdateHorseAsync(int id, int ownerUserId, UpdateHorseDto dto)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var horse = await _horseRepo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Horse), id);

        if (horse.HorseOwnerId != owner.Id)
            throw new ForbiddenException("You do not own this horse.");

        horse.Name = dto.Name;
        horse.Breed = dto.Breed;
        horse.Age = dto.Age;
        horse.Color = dto.Color;
        horse.Weight = dto.Weight;
        horse.Status = dto.Status;
        horse.MedicalHistory = dto.MedicalHistory;
        horse.ImageUrl = dto.ImageUrl;
        horse.UpdatedAt = DateTime.UtcNow;

        _horseRepo.Update(horse);
        await _uow.SaveChangesAsync();

        return await GetHorseByIdAsync(id);
    }

    public async Task DeleteHorseAsync(int id, int ownerUserId)
    {
        var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == ownerUserId)
            ?? throw new NotFoundException(nameof(HorseOwner), ownerUserId);

        var horse = await _horseRepo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Horse), id);

        if (horse.HorseOwnerId != owner.Id)
            throw new ForbiddenException("You do not own this horse.");

        _horseRepo.Remove(horse);
        await _uow.SaveChangesAsync();
    }
}
