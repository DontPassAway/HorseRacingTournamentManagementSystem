using AutoMapper;
using HorseRacing.Application.DTOs.Horses;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
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

    // Đã nâng cấp hàm này để nhận và xử lý search/status
    public async Task<PagedResponse<HorseDto>> GetAllHorsesAsync(int page, int pageSize, string? search = null, string? status = null)
    {
        var query = _horseRepo.Query()
            .Include(h => h.HorseOwner)
            .ThenInclude(o => o.User)
            .AsQueryable();

        // 1. Xử lý Lọc theo trạng thái (Status)
        if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
        {
            if (Enum.TryParse<HorseStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(h => h.Status == parsedStatus);
            }
        }

        // 2. Xử lý Tìm kiếm (Search) theo tên ngựa hoặc tên/username chủ ngựa
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(h =>
                h.Name.ToLower().Contains(lowerSearch) ||
                (h.HorseOwner != null && h.HorseOwner.User != null &&
                    (h.HorseOwner.User.FullName.ToLower().Contains(lowerSearch) ||
                     h.HorseOwner.User.Username.ToLower().Contains(lowerSearch)))
            );
        }

        int total = await query.CountAsync();

        // Nên thêm OrderBy(h => h.Id) hoặc CreatedAt để Database phân trang chính xác, không bị warning
        var items = await query
            .OrderByDescending(h => h.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

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
    public async Task<HorseDto> UpdateHorseStatusAsync(int id, int userId, HorseStatus status, bool isAdmin)
    {
        var horse = await _horseRepo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Horse), id);

        if (!isAdmin)
        {
            var owner = await _ownerRepo.FirstOrDefaultAsync(o => o.UserId == userId)
                ?? throw new NotFoundException(nameof(HorseOwner), userId);
            if (horse.HorseOwnerId != owner.Id)
                throw new ForbiddenException("You do not own this horse.");
        }

        horse.Status = status;
        horse.UpdatedAt = DateTime.UtcNow;
        _horseRepo.Update(horse);
        await _uow.SaveChangesAsync();
        return await GetHorseByIdAsync(id);
    }
}
